using System;

namespace Net.Torrent.Tracker.Common.Udp
{
    public interface IUdpPacketValidator
    {
        /// <summary>
        /// Detects response type
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns><see cref="UdpActions"/></returns>
        UdpActions DetectResponseType(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Detects request type
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <param name="connectionIdValidator">Validator for connection id and transaction id</param>
        /// <returns><see cref="UdpActions"/></returns>
        UdpActions DetectRequestType(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Validates connect request
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Valid or not</returns>
        bool ValidateConnectRequest(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Validates announce request
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Valid or not</returns>
        bool ValidateAnnounceRequest(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Validates scrape request
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Valid or not</returns>
        bool ValidateScrapeRequest(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Validate connect response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Valid or not</returns>
        bool ValidateConnectResponse(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Validate announce response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Valid or not</returns>
        bool ValidateAnnounceResponse(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Validate scrape response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Valid or not</returns>
        bool ValidateScrapeResponse(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Validate error response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Valid or not</returns>
        bool ValidateErrorResponse(ReadOnlySpan<byte> bytes);
    }
}
