// ReSharper disable InconsistentNaming

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

using SpecUnit;

namespace AspComet.Specifications
{
    [Concern("Handling HTTP requests")]
    public class synchronously : behaves_like_context_with_http_handler
    {
        private Exception exception;

        protected override void Because()
        {
            exception = ((MethodThatThrows) (() => cometHttpHandler.ProcessRequest(null))).GetException();
        }

        [Observation]
        public void should_fail_to_process_the_request()
        {
            exception.ShouldNotBeNull();
        }
    }

    [Concern("Handling HTTP requests")]
    public class aynchronously : behaves_like_context_with_http_handler
    {
        private IAsyncResult asyncResult;

        protected override void Context()
        {
            base.Context();
            this.request.Form["message"] = "{ }";
        }

        protected override void Because()
        {
            asyncResult = cometHttpHandler.BeginProcessRequest(this.context, null, null);
        }

        [Observation]
        public void should_return_an_async_result()
        {
            asyncResult.ShouldNotBeNull();
        }
    }
   

    [Concern("Handling HTTP requests")]
    public class aynchronously_with_a_message_field_which_is_an_object : behaves_like_context_with_http_handler
    {
        protected override void Context()
        {
            base.Context();

            this.request.Form["message"] = "{ }";
        }

        protected override void Because()
        {
            cometHttpHandler.BeginProcessRequest(this.context, null, null);
        }

        [Observation]
        public void should_pass_a_single_message_to_the_message_bus()
        {
            messageBus.AssertWasCalled(x => x.HandleMessages(null, null), x => x.Constraints(Property.Value("Length", 1), Is.Anything()));
        }
    }

    [Concern("Handling HTTP requests")]
    public class aynchronously_with_a_message_field_which_is_an_array_of_3_objects : behaves_like_context_with_http_handler
    {
        protected override void Context()
        {
            base.Context();

            this.request.Form["message"] = "[ {}, {}, {} ]";
        }

        protected override void Because()
        {
            cometHttpHandler.BeginProcessRequest(this.context, null, null);
        }

        [Observation]
        public void should_pass_3_messages_to_the_message_bus()
        {
            messageBus.AssertWasCalled(x => x.HandleMessages(null, null), x => x.Constraints(Property.Value("Length", 3), Is.Anything()));
        }
    }

    [Concern("Handling HTTP requests")]
    public class aynchronously_with_a_body_which_is_an_array_of_3_objects : behaves_like_context_with_http_handler
    {
        protected override void Context()
        {
            base.Context();

            byte[] jsonAsByteArray = "[ {}, {}, {} ]".ToCharArray().Select(c => (byte) c).ToArray();
            this.request.Stub(x => x.InputStream).Return(new MemoryStream(jsonAsByteArray));
        }

        protected override void Because()
        {
            cometHttpHandler.BeginProcessRequest(this.context, null, null);
        }

        [Observation]
        public void should_pass_3_messages_to_the_message_bus()
        {
            messageBus.AssertWasCalled(x => x.HandleMessages(null, null), x => x.Constraints(Property.Value("Length", 3), Is.Anything()));
        }
    }

    public class behaves_like_context_with_http_handler : ContextSpecification
    {
        protected CometHttpHandler cometHttpHandler;
        protected IMessageBus messageBus;
        protected HttpContextBase context;
        protected HttpRequestBase request;

        protected override void Context()
        {
            messageBus = MockRepository.GenerateMock<IMessageBus>();
            cometHttpHandler = new CometHttpHandler();
            request = MockRepository.GenerateStub<HttpRequestBase>();
            context = MockRepository.GenerateStub<HttpContextBase>();

            context.Stub(x => x.Request).Return(request);
            request.Stub(x => x.Form).Return(new NameValueCollection());
            Configuration.InitialiseHttpHandler.WithMessageBus(messageBus);
        }
    }
}
