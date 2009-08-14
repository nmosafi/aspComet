namespace AspComet.Eventing
{
    public interface ICancellableEvent : IEvent
    {
        bool Cancel { get; set; }
    }
}
