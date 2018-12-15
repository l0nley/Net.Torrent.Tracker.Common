using Net.Torrent.Tracker.Common;
using System;

namespace Net.Torrent.Tracker
{
    public class TorrentTracker
    {
        public AnnounceResponse ProcessAnnounce(AnnounceRequest request)
        {
            throw new NotImplementedException();
        }

        public ConnectResponse ProcessConnect(ConnectRequest request)
        {
            throw new NotImplementedException();
        }

        public ScrapeResponse ProcessScrape(ScrapeRequest request)
        {
            throw new NotImplementedException();
        }

        public TrackerStats GetStats()
        {
            throw new NotImplementedException();
        }
    }
}
