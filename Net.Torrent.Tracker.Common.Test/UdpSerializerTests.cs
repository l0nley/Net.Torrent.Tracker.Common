using Net.Torrent.Tracker.Common.Udp;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Net.Torrent.Tracker.Common.Test
{
    public class UdpSerializerTests
    {
        [Fact]
        public void SerializesConnectRequest()
        {
            var ser = GetSerializer();
            var packet = ser.Serialize(new ConnectRequest(0x6f));
            Assert.Equal(SharedData.ConnectRequest, packet);
        }

        [Fact]
        public void SerializesConnectResponse()
        {
            var connectResponse = new ConnectResponse(0x6f, 0x7f);
            var response = GetSerializer().Serialize(connectResponse);
            Assert.Equal(SharedData.ConnectResponse, response);
        }

        [Fact]
        public void SerializesAnnounceRequest()
        {
            var hash = new byte[20];
            hash[18] = 0xfb;
            hash[19] = 0xff;
            var peerId = new byte[20];
            peerId[18] = 0xfc;
            peerId[19] = 0xff;
            var announcement = new AnnounceRequest(hash, peerId, 0xb268,
                new State(0, 0, 0), 0, 0xc8, EventType.Completed, null, 0xfaff, 0xffff);
            var ser = GetSerializer();
            var request = ser.Serialize(announcement);
            Assert.Equal(SharedData.AnnounceRequest, request);
        }

        [Fact]
        public void SerializesAnnounceResponse()
        {
            var peers = new List<Peer>(2)
            {
                new Peer(IPAddress.Parse("192.168.1.3"), 250),
                new Peer(IPAddress.Parse("192.168.1.6"), 250)
            };
            var response = new AnnounceResponse(0x06a8, peers.AsReadOnly(), null, 0xfa, 0x1, 0xff, null);
            var serialized = GetSerializer().Serialize(response);
            Assert.Equal(SharedData.AnnounceResponse, serialized);
        }

        [Fact]
        public void SerializesScrapeRequest()
        {
            var hashes = new ReadOnlySpan<byte>(SharedData.ScrapeRequest).Slice(16, 40).ToArray();
            var ser = GetSerializer();
            var request = ser.Serialize(new ScrapeRequest(0xfaff, 0xff, hashes, 2));
            Assert.Equal(SharedData.ScrapeRequest, request);
        }

        [Fact]
        public void SerializesScrapeResponse()
        {
            var lst = new List<ScrapeInfo>(1)
            {
               new ScrapeInfo(0x05, 0x07, 0x06)
            };

            var scrapeResponse = new ScrapeResponse(lst.AsReadOnly(), 0xfaff);
            var serialized = GetSerializer().Serialize(scrapeResponse);
            Assert.Equal(SharedData.ScrapeResponse, serialized);
        }

        [Fact]
        public void SerializesErrorResponse()
        {
            var err = new ErrorResponse(0xfa, "some text");
            var serialized = GetSerializer().Serialize(err);
            Assert.Equal(SharedData.ErrorResponse, serialized);
        }

        [Fact]
        public void DeserializeErrorResponse()
        {
            var error = GetSerializer().DeserializeErrorResponse(SharedData.ErrorResponse);
            Assert.Equal(0xfa, error.TransactionId);
            Assert.Equal("some text", error.ErrorMessage);
        }

        [Fact]
        public void DeserializesConnectRequest()
        {
            var req = GetSerializer().DeserializeConnectRequest(SharedData.ConnectRequest);
            Assert.Equal(0x6f, req.TransactionId);
        }

        [Fact]
        public void DeserializesConnectResponse()
        {
            var ser = GetSerializer();
            var response = ser.DeserializeConnectResponse(SharedData.ConnectResponse);
            Assert.Equal(0x7f, response.TransactionId);
            Assert.Equal(0x6f, response.ConnectionId);
        }

        [Fact]
        public void DeserializesAnnounceRequest()
        {
            var span = new ReadOnlySpan<byte>(SharedData.AnnounceRequest);
            var req = GetSerializer().DeserializeAnnounceRequest(SharedData.AnnounceRequest);
            Assert.Equal(0xfaff, req.ConnectionId);
            Assert.Equal(EventType.Completed, req.Event);
            Assert.Equal(0, req.Key);
            Assert.Equal(0xc8, req.NumWant);
            Assert.Equal(span.Slice(36, 20).ToArray(), req.PeerId);
            Assert.Equal(span.Slice(16, 20).ToArray(), req.Hash);
            Assert.Null(req.IPAddress);
            Assert.Equal(0xffff, req.TransactionId);
            Assert.Equal(0xb268, req.Port);
            Assert.Equal(0x00, req.State.Downloaded);
            Assert.Null(req.State.Corrupt);
            Assert.Equal(0x00, req.State.Left);
            Assert.Equal(0x00, req.State.Uploaded);
        }

        [Fact]
        public void DeserializesAnnounceResponse()
        {
            var ser = GetSerializer();
            var response = ser.DeserializerAnnounceResponse(SharedData.AnnounceResponse);
            Assert.Equal(0x06a8, response.Interval);
            Assert.Equal(0x01, response.Leechers);
            Assert.Equal(0xfa, response.Seeders);
            Assert.Equal(2, response.Peers.Count);
        }

        [Fact]
        public void DeserializesScrapeRequest()
        {
            var req = GetSerializer().DeserializeScrapeRequest(SharedData.ScrapeRequest);
            Assert.Equal(0xfaff, req.ConnectionId);
            Assert.Equal(0xff, req.TransactionId);
            Assert.Equal(2, req.HashCount);
        }

        [Fact]
        public void DeserializesScrapeResponse()
        {
            var ser = GetSerializer();
            var request = ser.DeserializeScrapeResponse(SharedData.ScrapeResponse);
            Assert.Equal(0xfaff, request.TransactionId);
            Assert.Equal(1, request.Info.Count);
            Assert.Equal(0x05, request.Info[0].Seeders);
            Assert.Equal(0x06, request.Info[0].Completed);
            Assert.Equal(0x07, request.Info[0].Leechers);
        }



        private IUdpSerializer GetSerializer()
        {
            var util = new DefaultUdpUtillity();
            var validator = new DefaultUdpPacketValidator(util, false);
            return new DefaultUdpSerializer(util, false, validator);
        }
    }
}
