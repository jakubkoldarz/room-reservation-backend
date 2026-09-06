using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("jobs_queue");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.JobType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(j => j.Payload)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(j => j.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(j => j.JobType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasIndex(j => new { j.Status, j.NextAttemptAt });
        }
    }
}
