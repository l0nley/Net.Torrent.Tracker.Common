using System.Collections.Generic;

namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Announce response from the tracker
    /// </summary>
    public readonly struct AnnounceResponse
    {
        /// <summary>
        /// Failure reason. Sometimes trackers returns additional information about errors/state.
        /// Only HTTP-based trackers sets this field
        /// </summary>
        public string FailureReason { get; }

        /// <summary>
        /// Interval, between announces
        /// </summary>
        public int Interval { get; }

        /// <summary>
        /// Optional. Some trackers also returns minimal interval between announcements
        /// </summary>
        public int? MinInterval { get; }

        /// <summary>
        /// Number of seeders
        /// </summary>
        public int? Seeders { get; }

        /// <summary>
        /// Number of leechers
        /// </summary>
        public int? Leechers { get; }

        /// <summary>
        /// Transaction id. Only set by UDP-based trackers
        /// </summary>
        public int? Transaction { get; }

        /// <summary>
        /// List of peers
        /// </summary>
        public IReadOnlyList<Peer> Peers { get; }

        /// <summary>
        /// Creates new <see cref="AnnounceResponse"/>
        /// </summary>
        /// <param name="interval">Interval between announces</param>
        /// <param name="peers">Peers list</param>
        /// <param name="minInterval">Minimal interval between announcements</param>
        /// <param name="seeders">Seeders</param>
        /// <param name="leechers">Leechers</param>
        /// <param name="transactionId">Transaction id, if exists</param>
        /// <param name="failReason">Fail reason string</param>
        public AnnounceResponse(int interval, IReadOnlyList<Peer> peers,
            int? minInterval = null, int? seeders = null, int? leechers = null, int? transactionId = null,
            string failReason = null)
        {
            Peers = peers;
            FailureReason = failReason;
            Interval = interval;
            MinInterval = minInterval;
            Seeders = seeders;
            Leechers = leechers;
            Transaction = transactionId;
        }
    }
}
