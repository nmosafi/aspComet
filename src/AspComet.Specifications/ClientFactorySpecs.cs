// ReSharper disable InconsistentNaming

using Machine.Specifications;

namespace AspComet.Specifications
{
    [Subject("Client factory")]
    public class when_creating_a_new_client
    {
        const string SpecifiedId = "TheID";
        static ClientFactory clientFactory;
        static IClient client;

        Establish context = () => 
            clientFactory = new ClientFactory();

        Because of = () =>
            client = clientFactory.CreateClient(SpecifiedId);

        It should_create_an_object_of_type_client = () =>
            client.ShouldBeOfType<Client>();

        It should_create_a_client_with_the_specified_id = () =>
            client.ID.ShouldEqual(SpecifiedId);
    }
}