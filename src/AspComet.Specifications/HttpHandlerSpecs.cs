// ReSharper disable InconsistentNaming

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

using Machine.Specifications;

using Microsoft.Practices.ServiceLocation;

using Rhino.Mocks;

namespace AspComet.Specifications
{
    [Subject("Handling HTTP requests")]
    public class synchronously : HttpHandlerScenario
    {
        static Exception exception;

        Because of =()=>
            exception = Catch.Exception(() => cometHttpHandler.ProcessRequest(null));

        It should_fail_to_process_the_request =()=> exception.ShouldNotBeNull();
    }

    [Subject("Handling HTTP requests")]
    public class asynchronously : HttpHandlerScenario
    {
        static IAsyncResult asyncResult;

        Establish context =()=>
            httpRequest.Form["message"] = "{ }";

        Because of =()=>
            asyncResult = cometHttpHandler.BeginProcessRequest(httpContext, null, null);

        It should_return_an_async_result =()=> asyncResult.ShouldNotBeNull();
    }
   
    [Subject("Handling HTTP requests")]
    public class asynchronously_with_a_message_field_which_is_a_single_object : HttpHandlerScenario
    {
        Establish context =()=>
            httpRequest.Form["message"] = "{ }";

        Because of =()=> 
            cometHttpHandler.BeginProcessRequest(httpContext, null, null);

        It should_pass_a_single_message_to_the_message_bus = () =>
            messageBus.ShouldHaveHadCalled(x => x.HandleMessages(Arg<Message[]>.Matches(m => m.Length == 1), Arg<CometAsyncResult>.Is.Anything));
    }

    [Subject("Handling HTTP requests")]
    public class asynchronously_with_a_message_field_which_is_an_array_of_3_objects : HttpHandlerScenario
    {
        static IAsyncResult asyncResult;
        
        Establish context = () =>
            httpRequest.Form["message"] = "[ {}, {}, {} ]";

        Because of =()=>
            asyncResult = cometHttpHandler.BeginProcessRequest(httpContext, null, null);

        It should_pass_3_messages_to_the_message_bus =()=> messageBus.ShouldHaveHadCalled(x => 
            x.HandleMessages(Arg<Message[]>.Matches(m => m.Length == 3), Arg<CometAsyncResult>.Is.Anything));

        It should_pass_async_result_to_the_message_bus = () => messageBus.ShouldHaveHadCalled(x =>
            x.HandleMessages(Arg<Message[]>.Is.Anything, Arg<CometAsyncResult>.Is.Equal(asyncResult)));
    }

    [Subject("Handling HTTP requests")]
    public class asynchronously_with_a_body_which_is_an_array_of_3_objects : HttpHandlerScenario
    {
        static IAsyncResult asyncResult;
        static readonly object asyncState = new object();
        static readonly AsyncCallback asyncCallback = delegate { };

        Establish context =()=>
        {
            byte[] jsonAsByteArray = "[ {}, {}, {} ]".Select(c => (byte) c).ToArray();
            httpRequest.Stub(x => x.InputStream).Return(new MemoryStream(jsonAsByteArray));
        };

        Because of =()=>
            asyncResult = cometHttpHandler.BeginProcessRequest(httpContext, asyncCallback, asyncState);

        It should_return_a_comet_async_result =()=> 
            asyncResult.ShouldBeOfType<CometAsyncResult>();

        It should_return_a_result_with_specified_state =()=> 
            asyncResult.AsyncState.ShouldEqual(asyncState);

        It should_pass_3_messages_to_the_message_bus =()=> messageBus.ShouldHaveHadCalled(x => 
            x.HandleMessages(Arg<Message[]>.Matches(m => m.Length ==  3), Arg<CometAsyncResult>.Is.Anything));

        It should_pass_async_result_to_the_message_bus =()=> messageBus.ShouldHaveHadCalled(x => 
            x.HandleMessages(Arg<Message[]>.Is.Anything, Arg<CometAsyncResult>.Is.Equal(asyncResult)));
    }

    [Subject("Handling HTTP requests")]
    public class when_completing_the_request : HttpHandlerScenario
    {
        static ICometAsyncResult asyncResult;

        Establish context =()=>
            asyncResult = MockRepository.GenerateStub<ICometAsyncResult>();

        Because of =()=>
            cometHttpHandler.EndProcessRequest(asyncResult);

        It should_send_awaiting_messages = () => 
            asyncResult.ShouldHaveHadCalled(x => x.SendAwaitingMessages());
    }

    public abstract class HttpHandlerScenario
    {
        protected static IServiceLocator serviceLocator; 
        protected static CometHttpHandler cometHttpHandler;
        protected static IMessageBus messageBus;
        protected static HttpContextBase httpContext;
        protected static HttpRequestBase httpRequest;

        Establish context =()=>
        {
            cometHttpHandler = new CometHttpHandler();
            messageBus = MockRepository.GenerateStub<IMessageBus>();
            httpRequest = MockRepository.GenerateStub<HttpRequestBase>();
            httpContext = MockRepository.GenerateStub<HttpContextBase>();
            serviceLocator = MockRepository.GenerateStub<IServiceLocator>();

            httpContext.Stub(x => x.Request).Return(httpRequest);
            httpRequest.Stub(x => x.Form).Return(new NameValueCollection());
            serviceLocator.Stub(x => x.GetInstance<IMessageBus>()).Return(messageBus);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        };
    }
}

