using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results;

namespace RoomReservation.Core.Services
{
    public class AuthService(IUserRepository _users, ITokenProvider _tokenProvider, IRefreshTokenService _refreshTokenService) : IAuthService
    {
        public async Task<Result<(string jwtToken, string refreshToken)>> LoginAsync(string email, string password, string? ipAddress, string? userAgent)
        {
            var user = await _users.GetUserByEmailAsync(email);
            if (user is null) 
                return Result<(string jwtToken, string refreshToken)>.Failure("Invalid credentials", ErrorType.BadRequest);

            var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordMatch)
                return Result<(string jwtToken, string refreshToken)>.Failure("Invalid credentials", ErrorType.BadRequest);

            var tokenResult = await _refreshTokenService.CreateTokenAsync(user.Id, ipAddress, userAgent);
            if(!tokenResult.IsSuccess) 
                return Result<(string jwtToken, string refreshToken)>.Failure(tokenResult.Error);

            var jwtToken = _tokenProvider.GenerateJwtToken(user);
            return Result<(string jwtToken, string refreshToken)>.Success((jwtToken, tokenResult.Value));
        }

        public async Task<Result<(string jwtToken, string refreshToken)>> RegisterAsync(string email, string password, string firstname, string lastname)
        {
            var user = await _users.GetUserByEmailAsync(email);
            if (user is not null) 
                return Result<(string jwtToken, string refreshToken)>.Failure("Email is already taken", ErrorType.BadRequest);

            var userToCreate = new User
            {
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            var createdUser = await _users.CreateAsync(userToCreate);
            var refreshToken = _tokenProvider.GenerateRefreshToken();

            var jwtToken = _tokenProvider.GenerateJwtToken(createdUser);
            return Result<(string jwtToken, string refreshToken)>.Success((jwtToken, refreshToken));
        }
    }
}
