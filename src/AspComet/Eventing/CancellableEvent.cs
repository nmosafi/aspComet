namespace AspComet.Eventing
{
    /// <summary>
    ///     Base class for an event which can be cancelled
    /// </summary>
    public abstract class CancellableEvent : ICancellableEvent
    {
        /// <summary>
        ///     Gets or sets whether to cancel the event
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        ///     Gets or sets sets the reason for cancellation
        /// </summary>
        public string CancellationReason { get; set; }
    }
}