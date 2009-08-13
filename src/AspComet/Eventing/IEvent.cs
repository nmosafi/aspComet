namespace AspComet.Eventing
{
    public interface IEvent
    {
        bool Cancel { get; set; }
    }
}