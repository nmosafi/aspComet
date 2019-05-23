using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http.Features;

namespace AspComet.Middleware
{
    public class CometMiddleware:IHttpBufferingFeature
    {

        public const int LongPollDurationInMilliseconds = 10000;
        public const int ClientTimeoutInMilliseconds = 20000;


        private IMessageBus msgBus;
        private HttpContext currentResponse;
        private DateTime startTime = DateTime.Now;

        // Must have constructor with this signature, otherwise exception at run time
        public CometMiddleware(RequestDelegate next)
        {
            // This is an HTTP Handler, so no need to store next
            // you've reached the end of the request pipeline!
        }

        public async Task Invoke(HttpContext context, IMessageBus messageBus)
        {
            currentResponse = context;
            ((IHttpBufferingFeature)this).DisableResponseBuffering();

            msgBus = messageBus;

            ////This snippet below shows ..
            ////How to wrap old Begin/End pattern (AMP) into the TAP pattern
            ////https://blog.stephencleary.com/2012/07/async-interop-with-iasyncresult.html
            var tcs = new TaskCompletionSource<bool>();
            IAsyncResult result = BeginProcessRequest(context, iar =>
            {
                try
                {
                    if (iar.IsCompleted)
                    {
                        EndProcessRequest(iar);
                        tcs.TrySetResult(true);
                    }
                }
                catch (OperationCanceledException) { tcs.TrySetCanceled(); }
                catch (Exception exc) { tcs.TrySetException(exc); }
            }, null);
            await tcs.Task;
            //System.Diagnostics.Trace.WriteLine($"OUTER Milli: {DateTime.Now.Subtract(this.startTime).TotalMilliseconds}");

            //while (DateTime.Now.Subtract(this.startTime).TotalMilliseconds < LongPollDurationInMilliseconds)
            //{
            //    // do the work in the loop
            //    System.Diagnostics.Trace.WriteLine($"Milli: {DateTime.Now.Subtract(this.startTime).TotalMilliseconds}");

            //    var tcs = new TaskCompletionSource<bool>();
            //    IAsyncResult result = BeginProcessRequest(context, iar =>
            //    {
            //        try
            //        {
            //            if (DateTime.Now.Subtract(this.startTime).TotalMilliseconds > LongPollDurationInMilliseconds)
            //            {
            //                EndProcessRequest(iar);
            //            }
            //            tcs.TrySetResult(true);
            //        }
            //        catch (OperationCanceledException) { tcs.TrySetCanceled(); }
            //        catch (Exception exc) { tcs.TrySetException(exc); }
            //    }, null);
            //    await tcs.Task;
            //}
        }

        private async Task DoWorkAsyncInfiniteLoop()
        {
            while (DateTime.Now.Subtract(this.startTime).TotalMilliseconds < LongPollDurationInMilliseconds)
            {
                // do the work in the loop
                BeginProcessRequest(this.currentResponse, null, null);

                // don't run again for at least 200 milliseconds
                await Task.Delay(200);
            }
        }

        /// <summary>
        /// original HttpHandler code - AMP pattern
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object asyncState)
        {
            Message[] request = MessageConverter.FromJson(context.Request);
            CometAsyncResult asyncResult = new CometAsyncResult(context, callback, asyncState);
            msgBus.HandleMessages(request, asyncResult);
            return asyncResult;
        }
        /// <summary>
        /// original HttpHandler code - AMP pattern
        /// </summary>
        /// <param name="result"></param>
        public void EndProcessRequest(IAsyncResult result)
        {
            ICometAsyncResult cometAsyncResult = (ICometAsyncResult)result;
            cometAsyncResult.SendAwaitingMessages();
        }

        public void DisableRequestBuffering()
        {
            throw new NotImplementedException();
        }

        public void DisableResponseBuffering()
        {
            IHttpBufferingFeature bufferingFeature = currentResponse.Features.Get<IHttpBufferingFeature>();
            if (bufferingFeature != null)
            {
                bufferingFeature.DisableResponseBuffering();
            }
        }
    }

    public static class CometExtensions
    {
        public static IApplicationBuilder UseCometMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CometMiddleware>();
        }

        /// <summary>
        /// the minimum set of required services (to be called from Startup.cs)
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureBasicCometServices(this IServiceCollection services)
        {
            //put AspComet registrations into the ServiceCollection container
            services.AddSingleton<IClientRepository, InMemoryClientRepository>();
            services.AddSingleton<IClientIDGenerator, RngUniqueClientIDGenerator>();
            services.AddSingleton<IClientFactory, ClientFactory>();
            services.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddTransient<IMessagesProcessor, MessagesProcessor>(messageProcessorCtorHelper);//per request might be AddScope? but that might be for more traditional app
            services.AddSingleton<IMessageBus, MessageBus>(messageBusCtorHelper);
            services.AddSingleton<IClientWorkflowManager,ClientWorkflowManager>();         
        }

        /// <summary>
        /// MessageBus has special delegate parameter in its ctor that I don't know 
        /// that standard Microsoft DI can handle without this helper function
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static MessageBus messageBusCtorHelper(IServiceProvider arg)
        {
            var repo = arg.GetService<IClientRepository>();
            return new MessageBus(repo, () => messageProcessorCtorHelper(arg));
        }

        /// <summary>
        /// It seems that Microsoft DI also needs help with MessagesProcessor's ctor, 
        /// probably because it's a nested call from the other helper above
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static MessagesProcessor messageProcessorCtorHelper(IServiceProvider arg)
        {
            var factory = arg.GetService<IMessageHandlerFactory>();
            return new MessagesProcessor(factory);
        }

    }
}