using System;
using System.Collections.Generic;

namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Scrape response
    /// </summary>
    public readonly struct ScrapeResponse
    {
        /// <summary>
        /// List of <see cref="ScrapeInfo"/>
        /// </summary>
        public IReadOnlyList<ScrapeInfo> Info { get; }

        /// <summary>
        /// Transaction id
        /// </summary>
        public int TransactionId { get; }

        /// <summary>
        /// Creates instance of <see cref="ScrapeResponse"/>
        /// </summary>
        /// <param name="infos">List of <see cref="ScrapeInfo"/></param>
        /// <param name="transactionId">Transaction id</param>
        /// <exception cref="ArgumentNullException">If <paramref name="infos"/> is null</exception>
        public ScrapeResponse(IReadOnlyList<ScrapeInfo> infos, int transactionId)
        {
            Info = infos ?? throw new ArgumentNullException(nameof(infos));
            TransactionId = transactionId;
        }
    }
}
