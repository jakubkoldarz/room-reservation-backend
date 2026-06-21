using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results;

namespace RoomReservation.Core.Services
{
    public class AuthService(IUserRepository _users, ITokenProvider _tokenProvider, IRefreshTokenService _refreshTokenService) : IAuthService
    {
        public async Task<ResultT<(string jwtToken, string refreshToken)>> LoginAsync(string email, string password, string? ipAddress, string? userAgent)
        {
            var user = await _users.GetUserByEmailAsync(email);
            if (user is null) 
                return ResultT<(string, string)>.Failure("Invalid credentials", ErrorType.BadRequest);

            var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordMatch)
                return ResultT<(string, string)>.Failure("Invalid credentials", ErrorType.BadRequest);

            var tokenResult = await _refreshTokenService.CreateTokenAsync(user.Id, ipAddress, userAgent);
            if(!tokenResult.IsSuccess) 
                return ResultT<(string, string)>.Failure(tokenResult.Error);

            await _refreshTokenService.DeleteExpiredAsync(user.Id);

            var jwtToken = _tokenProvider.GenerateJwtToken(user);
            return ResultT<(string, string)>.Success((jwtToken, tokenResult.Value));
        }

        public async Task<ResultT<(string jwtToken, string refreshToken)>> RegisterAsync(string email, string password, string firstname, string lastname)
        {
            var user = await _users.GetUserByEmailAsync(email);
            if (user is not null) 
                return ResultT<(string, string)>.Failure("Email is already taken", ErrorType.BadRequest);

            var userToCreate = new User
            {
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            var createdUser = await _users.CreateAsync(userToCreate);
            (var refreshToken, _) = _tokenProvider.GenerateRefreshToken();

            var jwtToken = _tokenProvider.GenerateJwtToken(createdUser);
            return ResultT<(string, string)>.Success((jwtToken, refreshToken));
        }
    }
}
