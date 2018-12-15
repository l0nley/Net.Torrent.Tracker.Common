using Net.Torrent.Tracker.Common.Udp;
using Xunit;

namespace Net.Torrent.Tracker.Common.Test
{
    public class UdpValidatorTest
    {
        [Fact]
        public void CorrectlyDetectsConnectRequest()
        {
            var type = GetValidator().DetectRequestType(SharedData.ConnectRequest);
            Assert.Equal(UdpActions.Connect, type);
        }

        [Fact]
        public void CorrectlyDetectsConnectResponse()
        {
            var type = GetValidator().DetectResponseType(SharedData.ConnectResponse);
            Assert.Equal(UdpActions.Connect, type);
        }

        [Fact]
        public void CorrectlyDetectsAnnounceRequest()
        {
            var type = GetValidator().DetectRequestType(SharedData.AnnounceRequest);
            Assert.Equal(UdpActions.Announce, type);
        }

        [Fact]
        public void CorrectlyDetectsAnnounceResponse()
        {
            var type = GetValidator().DetectResponseType(SharedData.AnnounceResponse);
            Assert.Equal(UdpActions.Announce, type);
        }

        [Fact]
        public void CorrectlyDetectsScrapeRequest()
        {
            var type = GetValidator().DetectRequestType(SharedData.ScrapeRequest);
            Assert.Equal(UdpActions.Scrape, type);
        }

        [Fact]
        public void CorrectlyDetectsScrapeResponse()
        {
            var type = GetValidator().DetectResponseType(SharedData.ScrapeResponse);
            Assert.Equal(UdpActions.Scrape, type);
        }

        [Fact]
        public void CorrectlyDetectsErrorResponse()
        {
            var type = GetValidator().DetectResponseType(SharedData.ErrorResponse);
            Assert.Equal(UdpActions.Error, type);
        }


        private IUdpPacketValidator GetValidator()
        {
            return new DefaultUdpPacketValidator(new DefaultUdpUtillity(), false, null);
        }
    }
}
