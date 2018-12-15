using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Net.Torrent.Tracker.Common.Udp
{
    public class DefaultUdpSerializer : IUdpSerializer
    {
        private const string MalformedFormat = "Supplied data is invalid";
        private readonly int _ipSize;
        private readonly IUdpUtillity _util;
        private readonly IUdpPacketValidator _validator;

        /// <summary>
        /// Creates new instance of <see cref="DefaultUdpSerializer"/>
        /// </summary>
        public DefaultUdpSerializer(IUdpUtillity utillity, bool isIPV6, IUdpPacketValidator validator = null)
        {
            _util = utillity ?? throw new ArgumentNullException(nameof(utillity));
            _validator = validator;
            _ipSize = isIPV6 ? 16 : 4;
        }

        /// <summary>
        /// Maximum hash count in scrape request
        /// </summary>
        public const int MaxHashesScrape = 74;

        /// <inheritdoc/>
        public byte[] Serialize(ConnectRequest request)
        {
            var bytes = new byte[16];
            var protoBytes = _util.GetBytes(request.ProtocolId);
            var tranId = _util.GetBytes(request.TransactionId);
            Array.Copy(protoBytes, 0, bytes, 0, protoBytes.Length);
            Array.Copy(tranId, 0, bytes, 12, tranId.Length);
            return bytes;
        }

        /// <inheritdoc/>
        public byte[] Serialize(ConnectResponse response)
        {
            var resp = new byte[16];
            var tran = _util.GetBytes(response.TransactionId);
            var conn = _util.GetBytes(response.ConnectionId);
            Array.Copy(tran, 0, resp, 4, tran.Length);
            Array.Copy(conn, 0, resp, 8, conn.Length);
            return resp;
        }

        /// <inheritdoc/>
        public byte[] Serialize(AnnounceRequest request)
        {
            if (request.ConnectionId == null)
            {
                throw new ArgumentException("Connection id must be set", nameof(request));
            }
            if (request.TransactionId == null)
            {
                throw new ArgumentException("Transaction id must be set", nameof(request));
            }
            bool isIP6;
            byte[] ipValue;
            int adding;
            if (request.IPAddress == null)
            {
                isIP6 = false;
                ipValue = new byte[4];
                adding = 0;
            }
            else
            {
                ipValue = request.IPAddress.GetAddressBytes();
                if (request.IPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
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
            var buffer = _util.GetBytes(request.ConnectionId.Value);
            Array.Copy(buffer, 0, bytes, 0, buffer.Length);
            buffer = _util.GetBytes((int)UdpActions.Announce);
            Array.Copy(buffer, 0, bytes, 8, buffer.Length);
            buffer = _util.GetBytes(request.TransactionId.Value);
            Array.Copy(buffer, 0, bytes, 12, buffer.Length);
            Array.Copy(request.Hash, 0, bytes, 16, request.Hash.Length);
            Array.Copy(request.PeerId, 0, bytes, 36, request.PeerId.Length);
            buffer = _util.GetBytes(request.State.Downloaded);
            Array.Copy(buffer, 0, bytes, 56, buffer.Length);
            buffer = _util.GetBytes(request.State.Left);
            Array.Copy(buffer, 0, bytes, 64, buffer.Length);
            buffer = _util.GetBytes(request.State.Uploaded);
            Array.Copy(buffer, 0, bytes, 72, buffer.Length);
            buffer = _util.GetBytes((int)request.Event);
            Array.Copy(buffer, 0, bytes, 80, buffer.Length);
            Array.Copy(ipValue, 0, bytes, 84, isIP6 ? 16 : 4);
            buffer = _util.GetBytes(request.Key);
            Array.Copy(buffer, 0, bytes, 88 + adding, buffer.Length);
            buffer = _util.GetBytes(request.NumWant);
            Array.Copy(buffer, 0, bytes, 92 + adding, buffer.Length);
            buffer = _util.GetBytes(request.Port);
            Array.Copy(buffer, 0, bytes, 96 + adding, buffer.Length);
            return bytes;
        }

        /// <inheritdoc/>
        public byte[] Serialize(AnnounceResponse response)
        {
            var result = new byte[20 + response.Peers.Count * (_ipSize + 2)];
            var buf = _util.GetBytes((int)UdpActions.Announce);
            Array.Copy(buf, 0, result, 0, buf.Length);
            buf = _util.GetBytes(response.Transaction.GetValueOrDefault());
            Array.Copy(buf, 0, result, 4, buf.Length);
            buf = _util.GetBytes(response.Interval);
            Array.Copy(buf, 0, result, 8, buf.Length);
            buf = _util.GetBytes(response.Leechers.GetValueOrDefault());
            Array.Copy(buf, 0, result, 12, buf.Length);
            buf = _util.GetBytes(response.Seeders.GetValueOrDefault());
            Array.Copy(buf, 0, result, 16, buf.Length);
            for(var i=0; i< response.Peers.Count;i++)
            {
                var peer = response.Peers[i];
                buf = peer.Address.GetAddressBytes();
                if(BitConverter.IsLittleEndian)
                {
                    Array.Reverse(buf);
                }
                Array.Copy(buf, 0, result, 20 + (_ipSize + 2) * i, buf.Length);
                buf = _util.GetBytes(peer.Port);
                Array.Copy(buf, 0, result, 20 + _ipSize + (_ipSize + 2) * i, buf.Length);
            }

            return result;
        }

        /// <inheritdoc/>
        public byte[] Serialize(ScrapeRequest request)
        {
            if (request.Hashes.Length < request.HashCount * 20)
            {
                throw new ArgumentException($"Size of the hashes should be greater than 20*hashCount", nameof(request));
            }

            if (request.HashCount > MaxHashesScrape)
            {
                throw new ArgumentOutOfRangeException(nameof(request), $"hashCount cannot exceed {MaxHashesScrape}");
            }
            var bytes = new byte[16 + 20 * request.HashCount];
            var connectionIdBytes = _util.GetBytes(request.ConnectionId);
            var actionBytes = _util.GetBytes((int)UdpActions.Scrape);
            var tranBytes = _util.GetBytes(request.TransactionId);
            Array.Copy(connectionIdBytes, 0, bytes, 0, connectionIdBytes.Length);
            Array.Copy(actionBytes, 0, bytes, 8, actionBytes.Length);
            Array.Copy(tranBytes, 0, bytes, 12, tranBytes.Length);
            var hashes = new ReadOnlySpan<byte>(request.Hashes).Slice(0, request.HashCount * 20);
            for (var i = 0; i < request.HashCount; i++)
            {
                var arr = hashes.Slice(i * 20, 20).ToArray();
                Array.Copy(arr, 0, bytes, 16 + i * 20, 20);
            }
            return bytes;
        }

        /// <inheritdoc/>
        public byte[] Serialize(ScrapeResponse response)
        {
            var bytes = new byte[8 + response.Info.Count * 12];
            var buf = _util.GetBytes((int)UdpActions.Scrape);
            Array.Copy(buf, 0, bytes, 0, buf.Length);
            buf = _util.GetBytes(response.TransactionId);
            Array.Copy(buf, 0, bytes, 4, buf.Length);
            for (var i = 0; i < response.Info.Count; i++)
            {
                buf = _util.GetBytes(response.Info[i].Seeders);
                Array.Copy(buf, 0, bytes, 8 + 12 * i, buf.Length);
                buf = _util.GetBytes(response.Info[i].Completed);
                Array.Copy(buf, 0, bytes, 12 + 12 * i, buf.Length);
                buf = _util.GetBytes(response.Info[i].Leechers);
                Array.Copy(buf, 0, bytes, 16 + 12 * i, buf.Length);
            }
            return bytes;
        }

        /// <inheritdoc/>
        public byte[] Serialize(ErrorResponse response)
        {
            var ascii = response.ErrorMessage == null ? new byte[0] : Encoding.ASCII.GetBytes(response.ErrorMessage);
            var result = new byte[8 + ascii.Length];
            var action = _util.GetBytes((int)UdpActions.Error);
            var tran = _util.GetBytes(response.TransactionId);
            Array.Copy(action, 0, result, 0, action.Length);
            Array.Copy(tran, 0, result, 4, tran.Length);
            Array.Copy(ascii, 0, result, 8, ascii.Length);
            return result;
        }

        /// <inheritdoc/>
        public T Deserialize<T>(ReadOnlySpan<byte> bytes)
        {
            var t = typeof(T);
            if (t == typeof(ConnectRequest))
            {
                return (T)(object)DeserializeConnectRequest(bytes);
            }
            else if (t == typeof(ConnectResponse))
            {
                return (T)(object)DeserializeConnectResponse(bytes);
            }
            else if (t == typeof(AnnounceRequest))
            {
                return (T)(object)DeserializeAnnounceRequest(bytes);
            }
            else if (t == typeof(AnnounceResponse))
            {
                return (T)(object)DeserializerAnnounceResponse(bytes);
            }
            else if (t == typeof(ScrapeRequest))
            {
                return (T)(object)DeserializeScrapeRequest(bytes);
            }
            else if (t == typeof(ScrapeResponse))
            {
                return (T)(object)DeserializeScrapeResponse(bytes);
            }
            else if (t == typeof(ErrorResponse))
            {
                return (T)(object)DeserializeErrorResponse(bytes);
            }
            else
            {
                throw new NotSupportedException();
            }

        }

        /// <inheritdoc/>
        public ConnectRequest DeserializeConnectRequest(ReadOnlySpan<byte> bytes)
        {
            if (_validator != null && !_validator.ValidateConnectRequest(bytes))
            {
                throw new ArgumentException(MalformedFormat);
            }

            var tranId = _util.GetInt(bytes.Slice(12, 4));
            return new ConnectRequest(tranId);
        }

        /// <inheritdoc/>
        public ConnectResponse DeserializeConnectResponse(ReadOnlySpan<byte> bytes)
        {
            if (_validator != null && !_validator.ValidateConnectResponse(bytes))
            {
                throw new ArgumentException(MalformedFormat);
            }
            var transactionId = _util.GetInt(bytes.Slice(4, 4));
            var connectionId = _util.GetLong(bytes.Slice(8, 8));
            return new ConnectResponse(connectionId, transactionId);
        }

        /// <inheritdoc/>
        public AnnounceRequest DeserializeAnnounceRequest(ReadOnlySpan<byte> bytes)
        {
            if (_validator != null && !_validator.ValidateAnnounceRequest(bytes))
            {
                throw new ArgumentException(MalformedFormat);
            }

            var connId = _util.GetLong(bytes.Slice(0, 8));
            var tran = _util.GetInt(bytes.Slice(12, 4));
            var hash = bytes.Slice(16, 20).ToArray();
            var peer = bytes.Slice(36, 20).ToArray();
            var downloaded = _util.GetLong(bytes.Slice(56, 8));
            var left = _util.GetLong(bytes.Slice(64, 8));
            var uploaded = _util.GetLong(bytes.Slice(72, 8));
            var @event = _util.GetInt(bytes.Slice(80, 4));
            var ipAddr = _util.GetInt(bytes.Slice(84, _ipSize));
            IPAddress addr = null;
            if (ipAddr != 0x0000)
            {
                var arr = bytes.Slice(84, _ipSize).ToArray();
                if (_ipSize > 4)
                {
                    addr = new IPAddress(arr, 0xe);
                }
                else
                {
                    addr = new IPAddress(arr);
                }
            }
            var key = _util.GetInt(bytes.Slice(84 + _ipSize, 4));
            var numWant = _util.GetInt(bytes.Slice(88 + _ipSize, 4));
            var port = _util.GetShort(bytes.Slice(92 + _ipSize, 2));
            EventType evt = EventType.None;
            if (@event > 0 && @event < 4)
            {
                evt = (EventType)@event;
            }
            return new AnnounceRequest(hash, peer, (ushort)port, new State(uploaded, downloaded, left), key, numWant, evt,
               addr, connId, tran);
        }

        /// <inheritdoc/>
        public AnnounceResponse DeserializerAnnounceResponse(ReadOnlySpan<byte> bytes)
        {
            if (_validator != null && !_validator.ValidateAnnounceResponse(bytes))
            {
                throw new ArgumentException(MalformedFormat);
            }

            var tranId = _util.GetInt(bytes.Slice(4, 4));
            var interval = _util.GetInt(bytes.Slice(8, 4));
            var leechers = _util.GetInt(bytes.Slice(12, 4));
            var seeders = _util.GetInt(bytes.Slice(16, 4));
            IReadOnlyList<Peer> peers = null;
            if (bytes.Length > 20)
            {
                peers = Utils.ParsePeers(bytes, 20, _ipSize);
            }

            return new AnnounceResponse(interval, peers, seeders: seeders, leechers: leechers, transactionId: tranId);
        }

        /// <inheritdoc/>
        public ScrapeRequest DeserializeScrapeRequest(ReadOnlySpan<byte> bytes)
        {
            if (_validator != null && !_validator.ValidateScrapeRequest(bytes))
            {
                throw new ArgumentException(MalformedFormat);
            }

            var conId = _util.GetLong(bytes.Slice(0, 8));
            var tran = _util.GetInt(bytes.Slice(12, 4));
            var hashes = bytes.Slice(16).ToArray();
            return new ScrapeRequest(conId, tran, hashes, hashes.Length / 20);
        }

        /// <inheritdoc/>
        public ScrapeResponse DeserializeScrapeResponse(ReadOnlySpan<byte> bytes)
        {
            if (_validator != null && !_validator.ValidateScrapeResponse(bytes))
            {
                throw new ArgumentException(MalformedFormat);
            }

            var tran = _util.GetInt(bytes.Slice(4, 4));
            var lst = new List<ScrapeInfo>();
            for (var i = 8; i < bytes.Length; i += 12)
            {
                var seeders = _util.GetInt(bytes.Slice(i, 4));
                var completed = _util.GetInt(bytes.Slice(i + 4, 4));
                var leechers = _util.GetInt(bytes.Slice(i + 8, 4));
                lst.Add(new ScrapeInfo(seeders, leechers, completed));
            }

            return new ScrapeResponse(lst, tran);
        }

        /// <inheritdoc/>
        public ErrorResponse DeserializeErrorResponse(ReadOnlySpan<byte> bytes)
        {
            if (_validator != null && !_validator.ValidateErrorResponse(bytes))
            {
                throw new ArgumentException(MalformedFormat);
            }

            var tran = _util.GetInt(bytes.Slice(4, 4));
            var msg = Encoding.ASCII.GetString(bytes.Slice(8));
            return new ErrorResponse(tran, msg);
        }
    }
}
