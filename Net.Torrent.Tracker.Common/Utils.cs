using System;
using System.Collections.Generic;
using System.Net;

namespace Net.Torrent.Tracker.Common
{
    internal static class Utils
    {
        internal static IReadOnlyList<Peer> ParsePeers(ReadOnlySpan<byte> bytes, int startIndex, int ipSize)
        {
            if ((bytes.Length - startIndex) < ipSize + sizeof(short))
            {
                throw new DataMisalignedException("Invalid peer dictionary format");
            }
            var list = new List<Peer>(bytes.Length / (ipSize + sizeof(short)));
            for (var i = startIndex; i < bytes.Length; i += (ipSize + sizeof(short)))
            {
                IPAddress ip = null;
                var ipSlice = bytes.Slice(i, ipSize);
                if (ipSize > 4)
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
