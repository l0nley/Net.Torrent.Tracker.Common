using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Net.Torrent.Tracker.Common.Test
{
    public class TrackerTests
    {
        private const string HashString = "42015d75a42af5a1b20e1118bd0b71cddbafd5e8";
        private static readonly byte[] Hash = HexStringToBytes(HashString);
        private static readonly Uri Tracker = new Uri("http://bt2.t-ru.org/ann");

        [Fact]
        public void ConversionCorrect()
        {
            var convertedBack = BitConverter.ToString(Hash).Replace("-", string.Empty).ToLowerInvariant();
            Assert.Equal(HashString, convertedBack);
        }

        [Fact]
        public async Task PerformsRequest()
        {
            var peerId = GeneratePeerId();
            var announcement = new Announce(Hash, Encoding.ASCII.GetBytes(peerId), 6882, new State(0, 0, 0));
            using (var httpClient = new HttpClient())
            {
                var helper = new TrackerHelper();
                var request = helper.CreateHttpAnnounceRequest(Tracker, announcement);
                var response = await httpClient.SendAsync(request);
                Assert.True(response.IsSuccessStatusCode);
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var resp = helper.ParseHttpAnnounceResponse(bytes);
                Assert.True(resp.Peers.Count > 0);
            }
        }

        private string GeneratePeerId()
        {
            using (var sha1 = SHA1.Create())
            {
                return Encoding.ASCII.GetString(sha1.ComputeHash(Guid.NewGuid().ToByteArray()));
            }
        }

        private static byte[] HexStringToBytes(string s)
        {
            var span = HashString.AsSpan();
            var idx = 0;
            var bytes = new byte[s.Length / 2];
            for (var i = 0; i < span.Length - 1; i += 2)
            {
                var slice = span.Slice(i, 2);
                bytes[idx] = byte.Parse(slice, NumberStyles.AllowHexSpecifier);
                idx++;
            }
            return bytes;
        }
    }
}
