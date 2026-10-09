using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
    {
        public async Task<ResultT<User>> GetUserDetailsAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            return ResultT<User>.Success(user);
        }
        public async Task<ResultT<PagedList<User>>> GetAllAsync(UserFilter filters)
        {
            var users = await userRepository.GetFilteredAsync(filters);
            return ResultT<PagedList<User>>.Success(users);
        }
        public async Task<ResultT<User>> UpdateUserAsync(Guid userId, string firstname, string lastname)
        {
            var userToUpdate = await userRepository.GetByIdAsync(userId);
            if (userToUpdate is null)
                return new Error("User not found", ErrorType.NotFound);

            userToUpdate.Firstname = firstname;
            userToUpdate.Lastname = lastname;
            userToUpdate.IsProfileComplete = true;

            await unitOfWork.SaveChangesAsync();
            return ResultT<User>.Success(userToUpdate);
        }
    }
}
