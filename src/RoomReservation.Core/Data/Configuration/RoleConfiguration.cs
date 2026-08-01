using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RoomReservation.Core.Data.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> role)
        {
            role.HasKey(r => r.Id);
            role.HasIndex(r => r.Name).IsUnique();

            var allRoles = typeof(Roles)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(p => (RoleDefinition)p.GetValue(null)!);

            role.HasData(allRoles.Select(r =>
                new Role
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsDefault = r.IsDefault,
                    IsSuperAdmin = r.IsSuperAdmin,
                }));
        }
    }
}
