namespace Net.Torrent.Tracker.Common.Udp
{
    /// <summary>
    /// Udp action constants
    /// </summary>
    public enum UdpActions : int
    {
        /// <summary>
        /// Connect request/response
        /// </summary>
        Connect = 0,

        /// <summary>
        /// Announce request/response
        /// </summary>
        Announce = 1,

        /// <summary>
        /// Scrape request/response
        /// </summary>
        Scrape = 2,

        /// <summary>
        /// Error response
        /// </summary>
        Error = 3,

        /// <summary>
        /// Unknown action
        /// </summary>
        Unknown = int.MaxValue
    }
}
