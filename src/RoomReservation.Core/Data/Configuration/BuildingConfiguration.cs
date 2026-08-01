using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Data.Configuration
{
    public class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        public void Configure(EntityTypeBuilder<Building> building)
        {
            building.HasKey(b => b.Id);

            building.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);
            building.HasIndex(b => b.Name).IsUnique();

            building.Property(b => b.Identifier)
                .HasMaxLength(20);

            building.Property(b => b.Street)
                .IsRequired()
                .HasMaxLength(50);

            building.Property(b => b.City)
                .IsRequired()
                .HasMaxLength(50);

            building.Property(b => b.PostalCode)
               .IsRequired()
               .HasMaxLength(6);

            building.Property(b => b.FloorsCount)
               .IsRequired();
        }
    }
}
