namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Tracker connect response. Only generated during communication with UDP-based trackers
    /// </summary>
    public readonly struct ConnectResponse
    {
        /// <summary>
        /// Transaction id
        /// </summary>
        public int TransactionId { get; }

        /// <summary>
        /// Connection id
        /// </summary>
        public long ConnectionId { get; }

        /// <summary>
        /// Creates new instance <see cref="ConnectResponse"/>
        /// </summary>
        /// <param name="connectionId">Connection id</param>
        /// <param name="transactionId">Transaction id</param>
        public ConnectResponse(long connectionId, int transactionId)
        {
            TransactionId = transactionId;
            ConnectionId = connectionId;
        }
    }
}
