namespace Net.Torrent.Tracker.Common
{
    public readonly struct ErrorResponse
    {
        public int TransactionId { get; }
        public string ErrorMessage { get; }

        public ErrorResponse(int transactionId, string errorMessage)
        {
            TransactionId = transactionId;
            ErrorMessage = errorMessage;
        }
    }
}
