using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface ITokenProvider
    {
        (string token, string hash) GenerateRefreshToken();
        string GenerateJwtToken(User user);
    }
}
