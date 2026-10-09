using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class RoleService(IRoleRepository _roles, IUserRepository _users, IUnitOfWork _unitOfWork) : IRoleService
    {
        public async Task<Result> AssignRoleAsync(Guid roleId, Guid userId, Guid requestingUserId)
        {
            if(userId == requestingUserId)
                return new Error("You cannot assign a role to yourself.", ErrorType.BadRequest);

            var user = await _users.GetByIdAsync(userId);
            if(user is null)
                return new Error("User not found.", ErrorType.NotFound);

            var role = await _roles.GetByIdAsync(roleId);
            if(role is null)
                return new Error("Role not found.", ErrorType.NotFound);

            user.RoleId = roleId;
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<ResultT<Role>> CreateAsync(RoleModel request, bool force = false)
        {
            var result = await EnsureOnlyOneDefault(request, force);
            if (!result.IsSuccess)
                return result.Error;

            var toCreate = new Role
            {
                IsDefault = request.IsDefault,
                IsSuperAdmin = request.IsSuperAdmin,
                Name = request.Name,
            };
            toCreate.RolePermissions = [.. request.PermissionIds.Select(permissionId => new RolePermissions
            {
                PermissionId = permissionId,
                RoleId = toCreate.Id
            })];

            _roles.Add(toCreate);
            await _unitOfWork.SaveChangesAsync();

            return ResultT<Role>.Success(toCreate);
        }

        public async Task<Result> DeleteAsync(Guid roleId)
        {
            var toDelete = await _roles.GetByIdAsync(roleId);
            if (toDelete is null)
                return new Error("Role not found.", ErrorType.NotFound);

            var usersWithRole = await _users.GetFilteredAsync(new UserFilter { RoleId = roleId, Page = 1, PageSize = 1 });
            if(usersWithRole.TotalCount > 0)
                return new Error("Cannot delete role with assigned users.", ErrorType.Conflict);

            _roles.Remove(toDelete);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<PagedResult<Role>> GetAllAsync(RoleFilter filters)
        {
            var (Roles, TotalCount) = await _roles.GetFilteredAsync(filters);
            return PagedResult<Role>.Success(Roles, TotalCount, filters.Page, filters.PageSize);
        }

        public async Task<ResultT<Role>> GetByIdAsync(Guid roleId)
        {
            var role = await _roles.GetByIdAsync(roleId);
            if (role is null)
                return new Error("Role not found.", ErrorType.NotFound);
            return ResultT<Role>.Success(role);
        }

        public async Task<ResultT<Role>> UpdateAsync(Guid roleId, RoleModel request, bool force = false)
        {
            var toUpdate = await _roles.GetByIdAsync(roleId);
            if (toUpdate is null)
                return new Error("Role not found.", ErrorType.NotFound);

            var result = await EnsureOnlyOneDefault(request, force, toUpdate.Id);
            if (!result.IsSuccess)
                return result.Error;

            toUpdate.Name = request.Name;
            toUpdate.IsDefault = request.IsDefault;
            toUpdate.IsSuperAdmin = request.IsSuperAdmin;
            ReplacePermissions(toUpdate, request.PermissionIds);

            await _unitOfWork.SaveChangesAsync();
            return ResultT<Role>.Success(toUpdate);
        }

        private static void ReplacePermissions(Role role, IReadOnlyList<Guid> permissionIds)
        {
            var toRemove = role.RolePermissions.Where(rp => !permissionIds.Contains(rp.PermissionId)).ToList();
            foreach (var rolePermission in toRemove)
                role.RolePermissions.Remove(rolePermission);

            var toAdd = permissionIds.Where(id => role.RolePermissions.All(rp => rp.PermissionId != id));
            foreach (var permissionId in toAdd)
                role.RolePermissions.Add(new RolePermissions { PermissionId = permissionId, RoleId = role.Id });
        }

        private async Task<Result> EnsureOnlyOneDefault(RoleModel toCheck, bool force, Guid? excludeRoleId = null)
        {
            if (!toCheck.IsDefault) return Result.Success();

            var existingDefaultRole = await _roles.GetDefaultRoleAsync();
            if (existingDefaultRole is null || existingDefaultRole.Id == excludeRoleId)
                return Result.Success();

            if (!force)
                return new Error("A default role already exists.", ErrorType.Conflict);

            existingDefaultRole.IsDefault = false;
            return Result.Success();
        }
    }
}
