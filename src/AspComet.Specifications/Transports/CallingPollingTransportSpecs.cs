// ReSharper disable InconsistentNaming

using System.Web;

using AspComet.Transports;

using Machine.Specifications;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace AspComet.Specifications.Transports
{
    [Subject("Callback polling transport")]
    public class when_sending_messages : CallbackPollingTransportScenario
    {
        static readonly CallbackPollingTransport callbackPollingTransport = new CallbackPollingTransport();

        Because of = () =>
            callbackPollingTransport.SendMessages(httpResponse, httpRequest, messages);

        Behaves_like<ItCallsACallback> calls_a_callback;

        It should_call_a_javascript_function_called_jsonpcallback =()=>
            httpResponse.ShouldHaveHadCalled(x => x.Write(Arg<string>.Matches(new StartsWith("jsonpcallback"))));
    }

  /*
    [Subject("Callback polling transport")]
    public class when_sending_messages_and_a_custom_callback_name_has_been_specified : CallbackPollingTransportScenario
    {
        static readonly CallbackPollingTransport callbackPollingTransport = new CallbackPollingTransport("thisisafunction");

        Because of = () =>
            callbackPollingTransport.SendMessages(httpResponse, httpRequest, messages);

        Behaves_like<ItCallsACallback> calls_a_callback;

        It should_call_the_specified_javscript_function = () =>
            httpResponse.ShouldHaveHadCalled(x => x.Write(Arg<string>.Matches(new StartsWith("thisisafunction"))));
    }
   */

    [Behaviors]
    public class ItCallsACallback : CallbackPollingTransportScenario
    {
        It should_set_the_response_content_type_to_javascript = () =>
            httpResponse.ContentType.ShouldEqual("text/javascript");

        It should_write_the_json_for_the_message_as_the_argument_to_the_callback = () =>
            httpResponse.ShouldHaveHadCalled(x => x.Write(Arg<string>.Matches(new EndsWith(string.Format("( {0} )", messagesAsJson)))));
    }

    public abstract class CallbackPollingTransportScenario
    {
        protected static Message[] messages;
        protected static string messagesAsJson;
        protected static HttpResponseBase httpResponse;
        protected static HttpRequestBase httpRequest;

        Establish context = () =>
        {
            messages = new[]
            { 
                MessageBuilder.BuildWithRandomPropertyValues(),
                MessageBuilder.BuildWithRandomPropertyValues()
            };

            messagesAsJson = MessageConverter.ToJson(messages);
            httpResponse = MockRepository.GenerateStub<HttpResponseBase>();
        };
    }
}