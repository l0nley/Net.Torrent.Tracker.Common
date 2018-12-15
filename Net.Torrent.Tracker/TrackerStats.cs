namespace Net.Torrent.Tracker
{
    public struct TrackerStats
    {
        public long HashesCount { get; set; }
        public long AnnounceRequests { get; set; }
        public long ConnectRequests { get; set; }
        public long ScrapeRequests { get; set; }
        public long FailedRequests { get; set; }
    }
}
