namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Scrape information
    /// </summary>
    public readonly struct ScrapeInfo
    {
        /// <summary>
        /// Number of seeders
        /// </summary>
        public int Seeders { get; }

        /// <summary>
        /// Number of leechers
        /// </summary>
        public int Leechers { get; }

        /// <summary>
        /// Number of peers, in completed state
        /// </summary>
        public int Completed { get; }

        /// <summary>
        /// Creates new isntance if <see cref="ScrapeInfo"/>
        /// </summary>
        /// <param name="seeders">Number of seeders</param>
        /// <param name="leechers">Number of leechers</param>
        /// <param name="completed">Number of peers, in completed state</param>
        public ScrapeInfo(int seeders, int leechers, int completed)
        {
            Seeders = seeders;
            Leechers = leechers;
            Completed = completed;
        }
    }
}
