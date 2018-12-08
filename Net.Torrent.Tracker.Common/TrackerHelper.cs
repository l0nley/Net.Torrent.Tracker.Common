using Net.Torrent.BEncode;
using Net.Torrent.Tracker.Common.Http;
using Net.Torrent.Tracker.Common.Udp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Class, to create and parse request/responses from/to tracker
    /// </summary>
    public class TrackerHelper
    {
        private const string MalformedFormat = "Data has invalid length or aligment";

        /// <summary>
        /// Maximum hash count in scrape request
        /// </summary>
        public const int MaxHashesScrape = 74;

        /// <summary>
        /// User Agent header, used to create http requests to trackers
        /// </summary>
        public string UserAgent { get; set; } = "Net.Torrent/0.1a";

        /// <summary>
        /// Creates new http announce request
        /// </summary>
        /// <param name="baseUri">Tracker url</param>
        /// <param name="announcement">Announcement structure</param>
        /// <returns>Request message, to be send to the tracker</returns>
        /// <exception cref="NotSupportedException">If <paramref name="baseUri"/> is not HTTP or HTTPS based URI</exception>
        public HttpRequestMessage CreateHttpAnnounceRequest(Uri baseUri, Announce announcement)
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

            var uri = builder.ToString();
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Clear();
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("User-Agent", UserAgent);
            request.Headers.ConnectionClose = true;
            return request;
        }

        /// <summary>
        /// Parses http announce response from tracker
        /// </summary>
        /// <param name="bytes">Bytes to parse</param>
        /// <param name="isIPV6">Is IPV6 peers expected</param>
        /// <returns><see cref="AnnounceResponse"/></returns>
        /// <exception cref="DataMisalignedException">If peers section malformed</exception>
        public AnnounceResponse ParseHttpAnnounceResponse(ReadOnlySpan<byte> bytes, bool isIPV6 = false)
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
                peers = ParsePeers(keyBytes, 0, isIPV6);
            }

            return new AnnounceResponse(interval, peers, minInterval, failReason: failReason);
        }

        /// <summary>
        /// Creates UDP connect request
        /// </summary>
        /// <param name="transactionId">transaction id</param>
        /// <returns>Request to be send to the tracker</returns>
        public byte[] CreateUdpConnectRequest(int transactionId)
        {
            var bytes = new byte[16];
            var protoBytes = GetLongBytes(UdpConstants.ProtocolId);
            var tranId = GetIntBytes(transactionId);
            Array.Copy(protoBytes, 0, bytes, 0, protoBytes.Length);
            Array.Copy(tranId, 0, bytes, 12, tranId.Length);
            return bytes;
        }

        /// <summary>
        /// Creates UDP announce request
        /// </summary>
        /// <param name="connectionId">Connection id, obtained at connect phase</param>
        /// <param name="transactionId">Transaction id</param>
        /// <param name="peerId">Peer id</param>
        /// <param name="announcement"><see cref="Announce"/></param>
        /// <returns>Request to be send to the tracker</returns>
        public byte[] CreateUdpAnnounceRequest(long connectionId, int transactionId, ReadOnlySpan<byte> peerId, Announce announcement)
        {
            bool isIP6;
            byte[] ipValue;
            int adding;
            if (announcement.IPAddress == null)
            {
                isIP6 = false;
                ipValue = new byte[4];
                adding = 0;
            }
            else
            {
                ipValue = announcement.IPAddress.GetAddressBytes();
                if (announcement.IPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    isIP6 = true;
                    adding = 12;
                }
                else
                {
                    isIP6 = false;
                    adding = 0;
                }
            }
            var bytes = isIP6 ? new byte[110] : new byte[98];
            var buffer = GetLongBytes(connectionId);
            Array.Copy(buffer, 0, bytes, 0, buffer.Length);
            buffer = GetIntBytes((int)UdpAction.Announce);
            Array.Copy(buffer, 0, bytes, 8, buffer.Length);
            buffer = GetIntBytes(transactionId);
            Array.Copy(buffer, 0, bytes, 12, buffer.Length);
            Array.Copy(announcement.Hash, 0, bytes, 16, announcement.Hash.Length);
            Array.Copy(announcement.PeerId, 0, bytes, 36, announcement.PeerId.Length);
            buffer = GetLongBytes(announcement.State.Downloaded);
            Array.Copy(buffer, 0, bytes, 56, buffer.Length);
            buffer = GetLongBytes(announcement.State.Left);
            Array.Copy(buffer, 0, bytes, 64, buffer.Length);
            buffer = GetLongBytes(announcement.State.Uploaded);
            Array.Copy(buffer, 0, bytes, 72, buffer.Length);
            buffer = GetIntBytes((int)announcement.Event);
            Array.Copy(buffer, 0, bytes, 80, buffer.Length);
            Array.Copy(ipValue, 0, bytes, 84, isIP6 ? 16 : 4);
            buffer = GetIntBytes(announcement.Key);
            Array.Copy(buffer, 0, bytes, 88 + adding, buffer.Length);
            buffer = GetIntBytes(announcement.NumWant);
            Array.Copy(buffer, 0, bytes, 92 + adding, buffer.Length);
            buffer = GetShortBytes(announcement.Port);
            Array.Copy(buffer, 0, bytes, 96 + adding, buffer.Length);
            return bytes;
        }

        /// <summary>
        /// Creates UDP scrape request
        /// </summary>
        /// <param name="connectionId">Connection id, obtained at connect phase</param>
        /// <param name="transactionId">Transaction id</param>
        /// <param name="hashes">Hashes to scrape</param>
        /// <param name="hashCount">Amount of hashes to scrape</param>
        /// <returns>Request, to be send to the tracker</returns>
        /// <exception cref="ArgumentException">If <paramref name="hashes"/> length is less than hashCount*20</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="hashCount"/> exceed MaxHashesScrape</exception>
        public byte[] CreateUdpScrapeRequest(long connectionId, int transactionId, ReadOnlySpan<byte> hashes, int hashCount)
        {
            if (hashes.Length < hashCount * 20)
            {
                throw new ArgumentException($"Size of the hashes should be greater than 20*hashCount", nameof(hashes));
            }

            if (hashCount > MaxHashesScrape)
            {
                throw new ArgumentOutOfRangeException(nameof(hashCount), $"hashCount cannot exceed {MaxHashesScrape}");
            }

            var bytes = new byte[16 + 20 * hashCount];
            var connectionIdBytes = GetLongBytes(connectionId);
            var actionBytes = GetIntBytes((int)UdpAction.Scrape);
            var tranBytes = GetIntBytes(transactionId);
            Array.Copy(connectionIdBytes, 0, bytes, 0, connectionIdBytes.Length);
            Array.Copy(actionBytes, 0, bytes, 8, actionBytes.Length);
            Array.Copy(tranBytes, 0, bytes, 12, tranBytes.Length);
            var arr = hashes.Slice(0, hashCount * 20).ToArray();
            for (var i = 0; i < hashCount; i++)
            {
                Array.Copy(arr, i * 20, bytes, 16 + i * 20, 20);
            }
            return bytes;
        }

        /// <summary>
        /// Parses udp connect response
        /// </summary>
        /// <param name="bytes">Bytes to parse</param>
        /// <returns><see cref="ConnectResponse"/></returns>
        /// <exception cref="ArgumentException">If <paramref name="bytes"/> contains malformed packet</exception>
        public ConnectResponse ParseUdpConnectResponse(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 16)
            {
                throw new ArgumentException(MalformedFormat);
            }
            var slice = bytes.Slice(0, 16);
            var action = GetInt(slice.Slice(0, 4));
            var transactionId = GetInt(slice.Slice(4, 4));
            var connectionId = GetLong(slice.Slice(8, 8));
            if (action != (int)UdpAction.Connect)
            {
                throw new ArgumentException(MalformedFormat);
            }
            return new ConnectResponse(connectionId, transactionId);
        }

        /// <summary>
        /// Parses udp announce response
        /// </summary>
        /// <param name="bytes">Bytes to parse</param>
        /// <param name="isIPV6">Is IPV6 peers expected</param>
        /// <returns><see cref="AnnounceResponse"/></returns>
        /// <exception cref="ArgumentException">If <paramref name="bytes"/> length is less than minimal or not expected</exception>
        /// <exception cref="ArgumentException">If detected action is not announce</exception>
        /// <exception cref="DataMisalignedException">If peers section is malformed</exception>
        public AnnounceResponse ParseUdpAnnounceResponse(ReadOnlySpan<byte> bytes, bool isIPV6 = false)
        {
            var ipSize = isIPV6 ? 16 : 4;
            if (bytes.Length < 20)
            {
                throw new ArgumentException(MalformedFormat);
            }

            if ((bytes.Length - 20) % (ipSize + 2) > 0)
            {
                throw new ArgumentException(MalformedFormat);
            }

            var action = GetInt(bytes.Slice(0, 4));
            if (action != (int)UdpAction.Announce)
            {
                throw new ArgumentException(MalformedFormat);
            }

            var tranId = GetInt(bytes.Slice(4, 4));
            var interval = GetInt(bytes.Slice(8, 4));
            var leechers = GetInt(bytes.Slice(12, 4));
            var seeders = GetInt(bytes.Slice(16, 4));
            IReadOnlyList<Peer> peers = null;
            if (bytes.Length > 20)
            {
                peers = ParsePeers(bytes, 20, isIPV6);
            }

            return new AnnounceResponse(interval, peers, seeders: seeders, leechers: leechers, transactionId: tranId);
        }

        /// <summary>
        /// Parses UDP scrape response
        /// </summary>
        /// <param name="bytes">Bytes to parse</param>
        /// <returns><see cref="ScrapeResponse"/></returns>
        /// <exception cref="ArgumentException">If <paramref name="bytes"/> contains malformed data</exception>
        public ScrapeResponse ParseUdpScrapeResponse(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 8)
            {
                throw new ArgumentException(MalformedFormat);
            }

            if ((bytes.Length - 8) % 12 > 0)
            {
                throw new ArgumentException(MalformedFormat);
            }

            var action = GetInt(bytes.Slice(0, 4));
            var tran = GetInt(bytes.Slice(4, 4));
            if (action != (int)UdpAction.Scrape)
            {
                throw new ArgumentException(MalformedFormat);
            }

            var lst = new List<ScrapeInfo>();
            for (var i = 8; i < bytes.Length; i += 12)
            {
                var seeders = GetInt(bytes.Slice(i, 4));
                var completed = GetInt(bytes.Slice(i + 4, 4));
                var leechers = GetInt(bytes.Slice(i + 8, 4));
                lst.Add(new ScrapeInfo(seeders, leechers, completed));
            }

            return new ScrapeResponse(lst, tran);
        }

        private byte[] GetShortBytes(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        private byte[] GetLongBytes(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        private byte[] GetIntBytes(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        private int GetInt(ReadOnlySpan<byte> bytes)
        {
            var value = BitConverter.ToInt32(bytes);
            return IPAddress.NetworkToHostOrder(value);
        }

        private long GetLong(ReadOnlySpan<byte> bytes)
        {
            var value = BitConverter.ToInt64(bytes);
            return IPAddress.NetworkToHostOrder(value);
        }

        private IReadOnlyList<Peer> ParsePeers(ReadOnlySpan<byte> bytes, int startIndex, bool isIPV6)
        {
            var ipSize = isIPV6 ? 16 : 4;
            if ((bytes.Length - startIndex) < ipSize + sizeof(short))
            {
                throw new DataMisalignedException("Invalid peer dictionary format");
            }
            var list = new List<Peer>(bytes.Length / (ipSize + sizeof(short)));
            for (var i = startIndex; i < bytes.Length; i += (ipSize + sizeof(short)))
            {
                IPAddress ip = null;
                var ipSlice = bytes.Slice(i, ipSize);
                if (isIPV6)
                {
                    ip = new IPAddress(ipSlice.ToArray(), 0xe);
                }
                else
                {
                    ip = new IPAddress(ipSlice.ToArray());
                }

                var port = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes.Slice(i + ipSize, sizeof(short))));
                list.Add(new Peer(ip, port));
            }

            return list;
        }
    }
}
