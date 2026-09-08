namespace RoomReservation.Core.Emails
{
    public class VerificationCodeEmail : EmailMessage
    {
        public required string Title { get; init; }
        public required string CodePurpose { get; init; }
        public required string Code { get; init;  }
        public required int ExpirationMinutes { get; init; }

        internal override string Subject => "Twój kod weryfikacyjny";

        internal override Dictionary<string, string> GetReplacements() => new()
        {
            [nameof(Title)] = Title,
            [nameof(Code)] = Code,
            [nameof(CodePurpose)] = CodePurpose,
            [nameof(ExpirationMinutes)] = ExpirationMinutes.ToString(),
        };
    }
}
