namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Peer state
    /// </summary>
    public readonly struct State
    {
        /// <summary>
        /// Uploaded bytes
        /// </summary>
        public long Uploaded { get; }

        /// <summary>
        /// Downloaded bytes
        /// </summary>
        public long Downloaded { get; }

        /// <summary>
        /// Bytes, left to complete
        /// </summary>
        public long Left { get; }

        /// <summary>
        /// Amounce of corrpted bytes
        /// </summary>
        public long? Corrupt { get; }

        /// <summary>
        /// Creates new instance of <see cref="State"/>
        /// </summary>
        /// <param name="uploaded">Uploaded bytes</param>
        /// <param name="downloaded">Downloaded bytes</param>
        /// <param name="left">Left bytes</param>
        /// <param name="corrupt">Corrupt bytes</param>
        public State(long uploaded, long downloaded, long left, long? corrupt = null)
        {
            Left = left;
            Uploaded = uploaded;
            Downloaded = downloaded;
            Corrupt = corrupt;
        }
    }
}
