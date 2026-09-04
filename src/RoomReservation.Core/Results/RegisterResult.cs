namespace RoomReservation.Core.Results
{
    public class RegisterResult
    {
        public Guid VerificationId { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
