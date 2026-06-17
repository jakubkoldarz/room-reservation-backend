using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Services
{
    public class AuthService(IUserRepository _users, ITokenProvider _tokenProvider) : IAuthService
    {
        public async Task<Result<string>> LoginAsync(string email, string password)
        {
            var user = await _users.GetUserByEmailAsync(email);
            if (user is null) return Result<string>.Failure("Invalid credentials");

            var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordMatch) return Result<string>.Failure("Invalid Credentials");

            var jwtToken = _tokenProvider.GenerateJwtToken(user);
            return Result<string>.Success(jwtToken);
        }

        public async Task<Result<string>> RegisterAsync(string email, string password, string firstname, string lastname)
        {
            var user = await _users.GetUserByEmailAsync(email);
            if (user is not null) return Result<string>.Failure("Email is already taken");

            var userToCreate = new User
            {
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            var createdUser = await _users.CreateAsync(userToCreate);

            var jwtToken = _tokenProvider.GenerateJwtToken(createdUser);
            return Result<string>.Success(jwtToken);
        }
    }
}
