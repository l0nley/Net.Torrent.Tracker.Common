namespace Net.Torrent.Tracker.Common
{
    public readonly struct ScrapeRequest
    {
        public long ConnectionId { get; }
        public int TransactionId { get; }
        public byte[] Hashes { get; }
        public int HashCount { get; }

        public ScrapeRequest(long connectionId, int transactionId, byte[] hashes, int hashCount)
        {
            ConnectionId = connectionId;
            TransactionId = transactionId;
            Hashes = hashes;
            HashCount = hashCount;
        }
    }
}
