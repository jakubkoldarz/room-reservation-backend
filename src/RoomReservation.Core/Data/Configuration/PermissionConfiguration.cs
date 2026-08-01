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

            var allPermissions = Permissions.Definitions.Select(p => new Permission{ Name = p.Key, Id = p.Value });
            builder.HasData(allPermissions.Select(p => new Permission { Id = p.Id, Name = p.Name }));
        }
    }
}
