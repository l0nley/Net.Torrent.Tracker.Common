using System;
using System.Net;

namespace Net.Torrent.Tracker.Common
{
    /// <summary>
    ///  Announce structure
    /// </summary>
    public readonly struct Announce
    {
        /// <summary>
        ///  Info has to announce
        /// </summary>
        public byte[] Hash { get; }

        /// <summary>
        /// Peer id
        /// </summary>
        public byte[] PeerId { get; }

        /// <summary>
        /// IP address of the peer
        /// </summary>
        public IPAddress IPAddress { get; }

        /// <summary>
        /// Port of the peer
        /// </summary>
        public ushort Port { get; }

        /// <summary>
        /// Current state
        /// </summary>
        public State State { get; }

        /// <summary>
        /// Number of peers want to receieve
        /// </summary>
        public int NumWant { get; }

        /// <summary>
        /// The key
        /// </summary>
        public int Key { get; }

        /// <summary>
        /// Event type
        /// </summary>
        public EventType Event { get; }


        /// <summary>
        /// Creates new <see cref="Announce"/>
        /// </summary>
        /// <param name="hash">Info hash</param>
        /// <param name="peerId">Peer id</param>
        /// <param name="port">Peer port</param>
        /// <param name="state">Donwloading state</param>
        /// <param name="key">The key</param>
        /// <param name="numWant">Number of peers to receiev</param>
        /// <param name="eventType">Event type</param>
        /// <param name="address">Peer address</param>
        /// <exception cref="ArgumentNullException">If <paramref name="hash"/>, <paramref name="peerId"/> is null</exception>
        /// <exception cref="ArgumentException">If <paramref name="hash"/> or <paramref name="peerId"/> have invalid length</exception>
        public Announce(byte[] hash, byte[] peerId, ushort port, State state,
            int key = 0, int numWant = -1, EventType eventType = EventType.None, IPAddress address = null)
        {

            Hash = hash ?? throw new ArgumentNullException(nameof(hash));
            if (hash.Length != 20)
            {
                throw new ArgumentException("Invalid length", nameof(hash));
            }
            PeerId = peerId ?? throw new ArgumentNullException(nameof(peerId));
            if (peerId.Length != 20)
            {
                throw new ArgumentException("Invalid length", nameof(peerId));
            }
            IPAddress = address;
            Port = port;
            State = state;
            Event = eventType;
            NumWant = numWant;
            Key = key;
        }
    }
}
