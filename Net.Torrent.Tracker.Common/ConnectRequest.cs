using Net.Torrent.Tracker.Common.Udp;

namespace Net.Torrent.Tracker.Common
{
    public readonly struct ConnectRequest
    {
        public long ProtocolId { get; }
        public int TransactionId { get; }

        public ConnectRequest(int transactionId)
        {
            TransactionId = transactionId;
            ProtocolId = UdpConstants.ProtocolId;
        }
    }
}
