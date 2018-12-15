using System;

namespace Net.Torrent.Tracker.Common.Http
{
    public interface IHttpSerializer
    {
        /// <summary>
        /// Creates Uri to announce 
        /// </summary>
        /// <param name="baseUri">base uri</param>
        /// <param name="request">the request</param>
        /// <returns>Uri to query</returns>
        Uri Serialize(Uri baseUri, AnnounceRequest request);

        /// <summary>
        /// Deserializes announce response
        /// </summary>
        /// <param name="bytes">Bytes of the response</param>
        /// <returns><see cref="AnnounceResponse"/></returns>
        AnnounceResponse Deserialize(ReadOnlySpan<byte> bytes);
    }
}
