using Net.Torrent.BEncode;
using System.Text;

namespace Net.Torrent.Tracker.Common.Http
{
    /// <summary>
    /// Http constants
    /// </summary>
    public static class HttpConstants
    {
        /// <summary>
        /// Interval dictionary key
        /// </summary>
        public static readonly BString IntervalKey = new BString("interval", Encoding.ASCII);

        /// <summary>
        /// Minimal interval dictionary key
        /// </summary>
        public static readonly BString MinIntervalKey = new BString("min interval", Encoding.ASCII);

        /// <summary>
        /// Peers dictionary key
        /// </summary>
        public static readonly BString PeersKey = new BString("peers", Encoding.ASCII);

        /// <summary>
        /// Fail reason dictionary key
        /// </summary>
        public static readonly BString FailReasonKey = new BString("failure reason", Encoding.ASCII);
    }
}
