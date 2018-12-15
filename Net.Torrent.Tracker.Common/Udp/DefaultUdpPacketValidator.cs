using System;

namespace Net.Torrent.Tracker.Common.Udp
{
    public class DefaultUdpPacketValidator : IUdpPacketValidator
    {
        private readonly IUdpUtillity _util;
        private readonly int _ipSize;
        private readonly Func<long, bool> _connectionIdValidator;

        public DefaultUdpPacketValidator(IUdpUtillity utillity, bool isIPV6, Func<long, bool> connectionIdValidator = null)
        {
            _util = utillity ?? throw new ArgumentNullException(nameof(utillity));
            _ipSize = isIPV6 ? 16 : 4;
            _connectionIdValidator = connectionIdValidator;
        }

        /// <inheritdoc/>
        public UdpActions DetectRequestType(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 8)
            {
                return UdpActions.Unknown;
            }

            var protoOrConnectionId = _util.GetLong(bytes.Slice(0, 8));
            if (protoOrConnectionId == UdpConstants.ProtocolId)
            {
                return ValidateConnectRequest(bytes) ? UdpActions.Connect : UdpActions.Unknown;
            }

            if (bytes.Length < 12)
            {
                return UdpActions.Unknown;
            }

            var action = _util.GetInt(bytes.Slice(8, 4));
            switch (action)
            {
                case (int)UdpActions.Announce:
                    return ValidateAnnounceRequest(bytes) ? UdpActions.Announce : UdpActions.Unknown;
                case (int)UdpActions.Scrape:
                    return ValidateScrapeRequest(bytes) ? UdpActions.Scrape : UdpActions.Unknown;
                default:
                    return UdpActions.Unknown;
            }
        }

        /// <inheritdoc/>
        public UdpActions DetectResponseType(ReadOnlySpan<byte> bytes)
        {

            if (bytes.Length < 4)
            {
                return UdpActions.Unknown;
            }

            var value = _util.GetInt(bytes.Slice(0, 4));
            switch (value)
            {
                case (int)UdpActions.Connect:
                    return ValidateConnectResponse(bytes) ? UdpActions.Connect : UdpActions.Unknown;
                case (int)UdpActions.Announce:
                    return ValidateAnnounceResponse(bytes) ? UdpActions.Announce : UdpActions.Unknown;
                case (int)UdpActions.Scrape:
                    return ValidateScrapeResponse(bytes) ? UdpActions.Scrape : UdpActions.Unknown;
                case (int)UdpActions.Error:
                    return ValidateErrorResponse(bytes) ? UdpActions.Error : UdpActions.Unknown;
                default:
                    return UdpActions.Unknown;
            }
        }

        /// <inheritdoc/>
        public bool ValidateConnectRequest(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 16)
            {
                return false;
            }
            var proto = _util.GetLong(bytes.Slice(0, 8));
            var action = _util.GetInt(bytes.Slice(8, 4));
            if (proto != UdpConstants.ProtocolId || action != (int)UdpActions.Connect)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ValidateAnnounceRequest(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 94 + _ipSize)
            {
                return false;
            }

            var connId = _util.GetLong(bytes.Slice(0, 8));
            var action = _util.GetInt(bytes.Slice(8, 4));
            if (action != (int)UdpActions.Announce)
            {
                return false;
            }

            if (_connectionIdValidator != null)
            {
                return _connectionIdValidator(connId);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ValidateScrapeRequest(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 16)
            {
                return false;
            }

            if ((bytes.Length - 16) % 20 > 0)
            {
                return false;
            }

            var connId = _util.GetLong(bytes.Slice(0, 8));
            var action = _util.GetInt(bytes.Slice(8, 4));
            if (action != (int)UdpActions.Scrape)
            {
                return false;
            }

            if (_connectionIdValidator != null)
            {
                return _connectionIdValidator(connId);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ValidateConnectResponse(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 16)
            {
                return false;
            }

            if (_util.GetInt(bytes.Slice(0, 4)) != (int)UdpActions.Connect)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ValidateAnnounceResponse(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 20)
            {
                return false;
            }

            if ((bytes.Length - 20) % (_ipSize + 2) > 0)
            {
                return false;
            }


            if (_util.GetInt(bytes.Slice(0, 4)) != (int)UdpActions.Announce)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ValidateScrapeResponse(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 8)
            {
                return false;
            }

            if ((bytes.Length - 8) % 12 > 0)
            {
                return false;
            }

            if (_util.GetInt(bytes.Slice(0, 4)) != (int)UdpActions.Scrape)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ValidateErrorResponse(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 8)
            {
                return false;
            }

            var action = _util.GetInt(bytes.Slice(0, 4));
            if (action != (int)UdpActions.Error)
            {
                return false;
            }

            return true;
        }

    }
}
