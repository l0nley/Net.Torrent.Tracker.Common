using System;
using System.Net;

namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    /// Peer 
    /// </summary>
    public readonly struct Peer
    {
        /// <summary>
        /// Peer address
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// Peer port
        /// </summary>
        public ushort Port { get; }

        /// <summary>
        /// Creates new instance of <see cref="Peer"/>
        /// </summary>
        /// <param name="address">Peer address</param>
        /// <param name="port">Peer port</param>
        /// <exception cref="ArgumentNullException">If <paramref name="address"/> is null</exception>
        public Peer(IPAddress address, ushort port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;
        }
    }
}
