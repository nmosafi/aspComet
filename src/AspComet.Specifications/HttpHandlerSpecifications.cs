// ReSharper disable InconsistentNaming

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

using Machine.Specifications;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace AspComet.Specifications
{
    [Subject("Handling HTTP requests")]
    public class synchronously : with_http_handler
    {
        static Exception exception;

        Because of =()=>
        {
            exception = GetException.From(() => cometHttpHandler.ProcessRequest(null));
        };

        It should_fail_to_process_the_request =()=> exception.ShouldNotBeNull();
    }

    [Subject("Handling HTTP requests")]
    public class aynchronously : with_http_handler
    {
        static IAsyncResult asyncResult;

        Establish context =()=>
            httpRequest.Form["message"] = "{ }";

        Because of =()=>
            asyncResult = cometHttpHandler.BeginProcessRequest(httpContext, null, null);

        It should_return_an_async_result =()=> asyncResult.ShouldNotBeNull();
    }
   
    [Subject("Handling HTTP requests")]
    public class aynchronously_with_a_message_field_which_is_an_object : with_http_handler
    {
        Establish context =()=>
        {
            httpRequest.Form["message"] = "{ }";
        };

        Because of =()=> 
            cometHttpHandler.BeginProcessRequest(httpContext, null, null);

        It should_pass_a_single_message_to_the_message_bus =()=>
            messageBus.AssertWasCalled(x => x.HandleMessages(null, null), 
                                       x => x.Constraints(Property.Value("Length", 1), Is.Anything()));
    }

    [Subject("Handling HTTP requests")]
    public class aynchronously_with_a_message_field_which_is_an_array_of_3_objects : with_http_handler
    {
        Establish context =()=>
            httpRequest.Form["message"] = "[ {}, {}, {} ]";

        Because of =()=>
            cometHttpHandler.BeginProcessRequest(httpContext, null, null);

        It should_pass_3_messages_to_the_message_bus =()=>
            messageBus.AssertWasCalled(x => x.HandleMessages(null, null), x => x.Constraints(Property.Value("Length", 3), Is.Anything()));
    }

    [Subject("Handling HTTP requests")]
    public class aynchronously_with_a_body_which_is_an_array_of_3_objects : with_http_handler
    {
        Establish context =()=>
        {
            byte[] jsonAsByteArray = "[ {}, {}, {} ]".ToCharArray().Select(c => (byte) c).ToArray();
            httpRequest.Stub(x => x.InputStream).Return(new MemoryStream(jsonAsByteArray));
        };

        Because of =()=>
            cometHttpHandler.BeginProcessRequest(httpContext, null, null);

        It should_pass_3_messages_to_the_message_bus =()=>
            messageBus.AssertWasCalled(x => x.HandleMessages(null, null), x => x.Constraints(Property.Value("Length", 3), Is.Anything()));
    }

    public abstract class with_http_handler
    {
        protected static CometHttpHandler cometHttpHandler;
        protected static IMessageBus messageBus;
        protected static HttpContextBase httpContext;
        protected static HttpRequestBase httpRequest;

        Establish context =()=>
        {
            messageBus = MockRepository.GenerateMock<IMessageBus>();
            cometHttpHandler = new CometHttpHandler();
            httpRequest = MockRepository.GenerateStub<HttpRequestBase>();
            httpContext = MockRepository.GenerateStub<HttpContextBase>();

            httpContext.Stub(x => x.Request).Return(httpRequest);
            httpRequest.Stub(x => x.Form).Return(new NameValueCollection());
            Configuration.InitialiseHttpHandler.WithMessageBus(messageBus);
        };
    }
}
