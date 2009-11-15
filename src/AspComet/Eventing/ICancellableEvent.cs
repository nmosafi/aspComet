namespace AspComet.Eventing
{
    /// <summary>
    ///     Provides an interface for events which can be cancelled
    /// </summary>
    public interface ICancellableEvent : IEvent
    {
        /// <summary>
        ///     Gets or sets whether to cancel the event
        /// </summary>
        bool Cancel { get; set; }

        /// <summary>
        ///     Gets or sets sets the reason for cancellation
        /// </summary>
        string CancellationReason { get; set; }
    }
}
