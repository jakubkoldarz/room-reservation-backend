namespace RoomReservation.Core.Emails
{
    public class PasswordChangeEmail : EmailMessage
    {
        public required string Title { get; init; }
        internal override string Subject => "Security alert";
        internal override Dictionary<string, string> GetReplacements() => new()
        {
            [nameof(Title)] = Title,
        };
    }
}
