namespace Net.Torrent.Tracker.Common.Udp
{
    /// <summary>
    /// Udp action constants
    /// </summary>
    public enum UdpAction : int
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
        Scrape = 2
    }
}
