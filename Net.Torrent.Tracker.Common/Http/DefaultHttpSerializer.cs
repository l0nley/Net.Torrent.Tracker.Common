using Net.Torrent.BEncode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Net.Torrent.Tracker.Common.Http
{
    public class DefaultHttpSerializer : IHttpSerializer
    {
        private readonly int _ipSize;

        public DefaultHttpSerializer(bool isIPV6)
        {
            _ipSize = isIPV6 ? 16 : 4;
        }

        public AnnounceResponse Deserialize(ReadOnlySpan<byte> bytes)
        {
            var offset = 0;
            var bencode = (BDictionary)new BEncodeSerializer().Deserialize(bytes, ref offset);
            int interval = -1;
            int? minInterval = null;
            string failReason = null;
            IReadOnlyList<Peer> peers = null;
            if (bencode.TryGetValue(HttpConstants.FailReasonKey, out IBEncodedObject value))
            {
                failReason = (BString)value;
            }

            if (bencode.TryGetValue(HttpConstants.IntervalKey, out value))
            {
                interval = (BNumber)value;
            }

            if (bencode.TryGetValue(HttpConstants.MinIntervalKey, out value))
            {
                minInterval = (BNumber)value;
            }

            if (bencode.TryGetValue(HttpConstants.PeersKey, out value))
            {
                var keyBytes = Encoding.ASCII.GetBytes(((BString)value).ToString(Encoding.ASCII));
                peers = Utils.ParsePeers(keyBytes, 0, _ipSize);
            }

            return new AnnounceResponse(interval, peers, minInterval, failReason: failReason);
        }

        /// <inheritdoc/>
        public Uri Serialize(Uri baseUri, AnnounceRequest announcement)
        {
            baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            if (!(string.Compare(baseUri.Scheme, "http", true) == 0 ||
                string.Compare(baseUri.Scheme, "https", true) == 0))
            {
                throw new NotSupportedException($"Request schema {baseUri.Scheme} not supported");
            }
            var query = new List<string>
            {
                    $"info_hash={HttpUtility.UrlEncode(announcement.Hash)}",
                    $"peer_id={HttpUtility.UrlEncode(announcement.PeerId)}",
                    $"port={announcement.Port}",
                    $"uploaded={announcement.State.Uploaded}",
                    $"downloaded={announcement.State.Downloaded}",
                    $"left={announcement.State.Left}",
                    $"num_want={announcement.NumWant}",
                    $"key={announcement.Key}",
                    "compact=1"
            };

            if (announcement.State.Corrupt != null)
            {
                query.Add($"corrupt={announcement.State.Corrupt.Value}");
            }

            if (announcement.IPAddress != null)
            {
                query.Add($"ip={announcement.IPAddress.ToString()}");
            }

            if (announcement.Event != EventType.None)
            {
                query.Add(Enum.GetName(typeof(EventType), announcement.Event).ToLowerInvariant());
            }

            var builder = new UriBuilder(baseUri)
            {
                Query = string.Join("&", query)
            };

            return builder.Uri;
        }
    }
}
