using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class AuthService(IUserRepository _users, ITokenProvider _tokenProvider) : IAuthService
    {
        public async Task<Result<string>> LoginAsync(string email, string password)
        {
            var user = await _users.GetUserByEmailAsync(email);
            if (user is null) return Result<string>.Failure("User was not found");

            var jwtToken = _tokenProvider.GenerateJwtToken();
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
                PasswordHash = password
            };

            await _users.CreateAsync(userToCreate);

            var jwtToken = _tokenProvider.GenerateJwtToken();
            return Result<string>.Success(jwtToken);
        }
    }
}
