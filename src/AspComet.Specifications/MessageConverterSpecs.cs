using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Machine.Specifications;

namespace AspComet.Specifications
{
    [Subject("Converting messages")]
    public class when_converting_a_message_with_no_properties
    {
        Because of =()=> json = MessageConverter.ToJson(new Message());

        It should_should_create_json_string_for_an_empty_javascript_object =()=> json.ShouldEqual("{}");

        static string json;
    }

    [Subject("Converting messages")]
    public class when_converting_a_message_with_a_null_property
    {
        Establish context =()=>
        {
            message = MessageBuilder.BuildWithRandomPropertyValues();
            message.error = null;
        };

        Because of = () => json = MessageConverter.ToJson(message);

        It should_should_strip_out_null_properties_from_json_string = () => json.ShouldNotContain(@"""error"":");

        static string json;
        static Message message;
    }

    [Subject("Converting messages")]
    public class when_converting_a_message_with_a_null_property_in_the_data
    {
        Establish context = () =>
        {
            message = MessageBuilder.BuildWithRandomPropertyValues();
            message.data = new { someproperty = (string) null };
        };

        Because of = () => json = MessageConverter.ToJson(message);

        It should_should_not_strip_out_the_null_property_from_json_string = () => json.ShouldContain(@"""someproperty"":null");

        static string json;
        static Message message;
    }

    [Subject("Extending Serializer")]
    public class when_using_the_default_serializer
    {
        It should_be_an_instance_of_JavaScriptSerializer = () => MessageConverter.Serializer().ShouldBeOfType(typeof (DefaultSerializer));
    }

    [Subject("Extending Serializer")]
    public class when_extending_the_with_a_different_serializer
    {
        Because of = () =>
        {
            MessageConverter.Serializer = () => new TestSerializer();
        };

        It should_be_an_instance_of_JavaScriptSerializer = () => MessageConverter.Serializer().ShouldBeOfType(typeof(TestSerializer));

        Cleanup reset_erializer = MessageConverter.ResetDefaultSerializer;

        class TestSerializer : ISerializer
        {
            public string Serialize(object obj)
            {
                throw new NotImplementedException();
            }

            public T Deserialize<T>(string json)
            {
                throw new NotImplementedException();
            }
        }
    }



}
