using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Entities;
using System.Reflection;

namespace RoomReservation.Core.Data.Configuration
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Name).IsUnique();

            var allPermissions = typeof(Permissions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(p => (PermissionDefinition)p.GetValue(null)!);

            builder.HasData(allPermissions.Select(p => new Permission { Id = p.Id, Name = p.Name }));
        }
    }
}
