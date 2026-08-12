using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Entities;
using System.Reflection;

namespace RoomReservation.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<BuildingAvailability> BuildingAvailabilities { get; set; }
        public DbSet<BuildingSpecialAvailability> BuildingSpecialAvailabilities { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomAvailability> RoomAvailabilities { get; set; }
        public DbSet<RoomSpecialAvailability> RoomSpecialAvailabilities { get; set; }
        public DbSet<RoomEquipment> RoomEquipment { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
