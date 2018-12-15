using System;

namespace Net.Torrent.Tracker.Common.Udp
{
    public interface IUdpUtillity
    {
        /// <summary>
        /// Returns bytes for short value
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>Bytes</returns>
        byte[] GetBytes(ushort value);

        /// <summary>
        /// Returns bytes for long value
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>Bytes</returns>
        byte[] GetBytes(long value);

        /// <summary>
        /// Returns bytes for int value
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>Bytes</returns>
        byte[] GetBytes(int value);

        /// <summary>
        /// Returns int from bytes
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Value</returns>
        int GetInt(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Returns short from bytes
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Value</returns>
        short GetShort(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Returns long from bytes
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Value</returns>
        long GetLong(ReadOnlySpan<byte> bytes);
    }
}
