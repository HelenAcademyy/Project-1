using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Helen.Repository;

namespace Helen.Domain.Invites.Response
{
    public class HelenDbContext : DbContext
    {
        public HelenDbContext(DbContextOptions<HelenDbContext> options)
            : base(options)
        {
        }

        public DbSet<LocationNotificationData> LocationNotificationData { get; set; }
        public DbSet<UserData> UserData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration for LocationNotificationData entity
            var locationEntity = modelBuilder.Entity<LocationNotificationData>();

            locationEntity.HasKey(p => p.Id);

            locationEntity.Property(p => p.Id)
                .ValueGeneratedOnAdd() // This ensures the Id is auto-incremented
                .IsRequired();

            locationEntity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(250);

            locationEntity.HasIndex(p => p.Name)
                .IsUnique();

            locationEntity.HasIndex(p => p.Type);
            locationEntity.HasIndex(p => p.Location);
            locationEntity.HasIndex(p => p.DateAdded);
            locationEntity.HasIndex(p => p.WeekdayOpenTime);
            locationEntity.HasIndex(p => p.WeekdayCloseTime);
            locationEntity.HasIndex(p => p.SaturdayOpenTime);
            locationEntity.HasIndex(p => p.SaturdayCloseTime);
            locationEntity.HasIndex(p => p.SundayOpenTime);
            locationEntity.HasIndex(p => p.SundayCloseTime);

            locationEntity.Property(p => p.Type)
                .HasMaxLength(250);

            locationEntity.Property(p => p.Location)
                .HasMaxLength(250);

            locationEntity.Property(p => p.ExtraInformation)
                .HasMaxLength(500);

            locationEntity.Property(p => p.DateAdded)
                .HasDefaultValueSql("GETDATE()");

            locationEntity.Property(p => p.WeekdayOpenTime)
                .HasColumnType("datetime2");

            locationEntity.Property(p => p.WeekdayCloseTime)
                .HasColumnType("datetime2");

            locationEntity.Property(p => p.SaturdayOpenTime)
                .HasColumnType("datetime2");

            locationEntity.Property(p => p.SaturdayCloseTime)
                .HasColumnType("datetime2");

            locationEntity.Property(p => p.SundayOpenTime)
                .HasColumnType("datetime2");

            locationEntity.Property(p => p.SundayCloseTime)
                .HasColumnType("datetime2");

            locationEntity.Property(p => p.Budget)
                .HasColumnType("decimal(18,2)");

            locationEntity.Property(p => p.RentPrice)
                .HasColumnType("decimal(18,2)");

            locationEntity.Property(p => p.AvailableForRent)
                .HasColumnType("bit");

            // Configuration for UserData entity
            var userEntity = modelBuilder.Entity<UserData>();

            userEntity.HasKey(u => u.Username);

            userEntity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(250);

            userEntity.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            userEntity.Property(u => u.Budget)
                .HasColumnType("decimal(18,2)");

            userEntity.Property(u => u.SendViaMail)
                .IsRequired();

            userEntity.HasIndex(u => u.Username)
                .IsUnique();

            // Ensure Id is auto-incremented for UserData if it exists
            if (userEntity.Metadata.FindProperty("Id") != null)
            {
                userEntity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
