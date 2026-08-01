using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Seeders
{
    public class DatabaseSeeder
    {
        private readonly List<ISeeder> seeders = [];
        private readonly AppDbContext context;

        public DatabaseSeeder(AppDbContext context)
        {
            this.context = context;
            seeders.Add(new UserSeeder(context));
        }

        public async Task SeedAsync()
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync();   
            }
        }
    }
}
