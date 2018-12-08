namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Event type
    /// </summary>
    public enum EventType : int
    {
        /// <summary>
        /// There are no event to send
        /// </summary>
        None = 0,

        /// <summary>
        /// Downloading started
        /// </summary>
        Started = 1,

        /// <summary>
        /// Downloading completed
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Downloading stopped
        /// </summary>
        Stopped = 3
    }
}
