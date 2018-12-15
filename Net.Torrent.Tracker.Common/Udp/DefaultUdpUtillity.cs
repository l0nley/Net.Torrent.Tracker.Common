using System;
using System.Net;

namespace Net.Torrent.Tracker.Common.Udp
{
    public class DefaultUdpUtillity : IUdpUtillity
    {
        /// <inheritdoc/>
        public byte[] GetBytes(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        /// <inheritdoc/>
        public byte[] GetBytes(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        /// <inheritdoc/>
        public byte[] GetBytes(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        /// <inheritdoc/>
        public int GetInt(ReadOnlySpan<byte> bytes)
        {
            var value = BitConverter.ToInt32(bytes);
            return IPAddress.NetworkToHostOrder(value);
        }

        /// <inheritdoc/>
        public short GetShort(ReadOnlySpan<byte> bytes)
        {
            var value = BitConverter.ToInt16(bytes);
            return IPAddress.NetworkToHostOrder(value);
        }

        /// <inheritdoc/>
        public long GetLong(ReadOnlySpan<byte> bytes)
        {
            var value = BitConverter.ToInt64(bytes);
            return IPAddress.NetworkToHostOrder(value);
        }

    }
}
