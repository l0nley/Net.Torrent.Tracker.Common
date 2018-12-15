using System;

namespace Net.Torrent.Tracker.Common.Udp
{
    public interface IUdpSerializer
    {
        /// <summary>
        /// Serializes connect request
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Serialized value</returns>
        byte[] Serialize(ConnectRequest request);

        /// <summary>
        /// Serializes connect response
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Serialized value</returns>
        byte[] Serialize(ConnectResponse response);

        /// <summary>
        /// Serializes announce request
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Serialized value</returns>
        byte[] Serialize(AnnounceRequest request);

        /// <summary>
        /// Serializes announce response
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Serialized value</returns>
        byte[] Serialize(AnnounceResponse response);

        /// <summary>
        /// Serializes scrape request
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Serialized value</returns>
        byte[] Serialize(ScrapeRequest request);

        /// <summary>
        /// Serializes scrape response
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Serialized value</returns>
        byte[] Serialize(ScrapeResponse response);

        /// <summary>
        /// Serializes error response
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Serialized value</returns>
        byte[] Serialize(ErrorResponse response);

        /// <summary>
        /// Deserializes packet
        /// </summary>
        /// <typeparam name="T">Type of packet</typeparam>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Instance of deserialized structure</returns>
        T Deserialize<T>(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Deserializes connect request
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Instance of the request</returns>
        ConnectRequest DeserializeConnectRequest(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Deserializes connect response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Instance of the response</returns>
        ConnectResponse DeserializeConnectResponse(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Deserializes announce request
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Instance of the request</returns>
        AnnounceRequest DeserializeAnnounceRequest(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Deserializes announce response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Instance of the response</returns>
        AnnounceResponse DeserializerAnnounceResponse(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Deserializes scrape request
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Instance of the request</returns>
        ScrapeRequest DeserializeScrapeRequest(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Deserializes scrape response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Instance of the response</returns>
        ScrapeResponse DeserializeScrapeResponse(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Deserializes error response
        /// </summary>
        /// <param name="bytes">The bytes</param>
        /// <returns>Instance of response</returns>
        ErrorResponse DeserializeErrorResponse(ReadOnlySpan<byte> bytes);
    }
}
