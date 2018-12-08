using System;
using Xunit;

namespace Net.Torrent.Tracker.Common.Test
{
    public class UdpParseGenTests
    {
        [Fact]
        public void GeneratesCorrectConnectPacket()
        {
            var expected = new byte[]
            {
                // protocol id
                0x00, 0x00, 0x04, 0x17, 0x27, 0x10, 0x19, 0x80,
                // action id
                0x00, 0x00, 0x00, 0x00,
                // transaction id
                0x00, 0x00, 0x00, 0x6f,
            };
            var packet = new TrackerHelper().CreateUdpConnectRequest(0x6f);
            Assert.Equal(expected, packet);
        }

        [Fact]
        public void ParsesCorrectConnectResponse()
        {
            var expected = new byte[]
            {
                // action  - connect 0
                0x00, 0x00, 0x00, 0x00,
                // tran id
                0x00, 0x00, 0x00, 0x7f,
                // connection id
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x6f
            };
            var response = new TrackerHelper().ParseUdpConnectResponse(expected);
            Assert.Equal(0x7f, response.TransactionId);
            Assert.Equal(0x6f, response.ConnectionId);
        }

        [Fact]
        public void CreatesCorrectAnnounceRequest()
        {
            var expected = new byte[]
            {
               // 0 connection id
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfa, 0xff,
               // 8 action = 1
               0x00, 0x00, 0x00, 0x01,
               // 12 transaction
               0x00, 0x00, 0xff, 0xff,
               // 16 info hash
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfb, 0xff,
               // 36 peer id
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfc, 0xff,
               // 56 downloaded
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
               // 64 left
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
               // 72 uploaded
               0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
               // 80 event (2)
               0x00, 0x00, 0x00, 0x02,
               // 84 ip addr
               0x00, 0x00, 0x00, 0x00,
               // 88 key
               0x00, 0x00, 0x00, 0x00,
               // 92 num want
               0x00, 0x00, 0x00, 0xc8,
               // 98 port
               0xb2, 0x68
            };
            var hash = new byte[20];
            hash[18] = 0xfb;
            hash[19] = 0xff;
            var peerId = new byte[20];
            peerId[18] = 0xfc;
            peerId[19] = 0xff;
            unchecked
            {
                var announcement = new Announce(hash, peerId, 0xb268,
                    new State(0, 0, 0), 0, 0xc8, EventType.Completed, null);
                var request = new TrackerHelper().CreateUdpAnnounceRequest(0x000000000000faff, 0x0000ffff, peerId, announcement);
                Assert.Equal(expected, request);
            }
        }

        [Fact]
        public void ParseCorrectAnnounceReponse()
        {
            var expected = new byte[]
            {
                // action
                0x00, 0x00, 0x00, 0x01,
                // transaction
                0x00, 0x00, 0x00, 0xff,
                // interval
                0x00, 0x00, 0x06, 0xa8,
                // leechers
                0x00, 0x00, 0x00, 0x01,
                // seeders
                0x00, 0x00, 0x00, 0xfa,
                // ip addr
                0xb0, 0x7a, 0x0e, 0x4b,
                // port
                0xc1, 0x94,
                // ip addr
                0xbc, 0xa3, 0x20, 0xd4,
                // port
                0x9e, 0x49
            };
            var response = new TrackerHelper().ParseUdpAnnounceResponse(expected, false);
            Assert.Equal(0x06a8, response.Interval);
            Assert.Equal(0x01, response.Leechers);
            Assert.Equal(0xfa, response.Seeders);
            Assert.Equal(2, response.Peers.Count);
        }

        [Fact]
        public void CreatesCorrectScrapeRequest()
        {
            var expected = new byte[]
            {
                // connection id
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfa, 0xff,
                // action
                0x00, 0x00, 0x00, 0x02,
                // transaction
                0x00, 0x00, 0x00, 0xff,
                // hash1
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfa, 0xff,
                // hash2
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xdd, 0xee
            };
            var hashes = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xfa, 0xff,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xdd, 0xee
            };
            var request = new TrackerHelper().CreateUdpScrapeRequest(0xfaff, 0xff, hashes, 2);
            Assert.Equal(expected, request);
        }

        [Fact]
        public void ParsesCorrectScrapeResponse()
        {
            var expected = new byte[]
            {
                // action
                0x00, 0x00, 0x00, 0x02,
                // transaction
                0x00, 0x00, 0xfa, 0xff,
                // seeders
                0x00, 0x00, 0x00, 0x05,
                // completed
                0x00, 0x00, 0x00, 0x06,
                // leechers
                0x00, 0x00, 0x00, 0x07
            };
            var request = new TrackerHelper().ParseUdpScrapeResponse(expected);
            Assert.Equal(0xfaff, request.TransactionId);
            Assert.Equal(1, request.Info.Count);
            Assert.Equal(0x05, request.Info[0].Seeders);
            Assert.Equal(0x06, request.Info[0].Completed);
            Assert.Equal(0x07, request.Info[0].Leechers);
        }
    }
}
