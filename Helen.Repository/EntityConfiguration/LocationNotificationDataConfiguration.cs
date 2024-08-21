using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Helen.Repository.Models;
using Helen.Domain.Invites.Response;

public class LocationNotificationDataConfiguration : IEntityTypeConfiguration<LocationNotificationData>
{
    public void Configure(EntityTypeBuilder<LocationNotificationData> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd() // This ensures the Id is auto-incremented
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.Location);
        builder.HasIndex(p => p.DateAdded);
        builder.HasIndex(p => p.WeekdayOpenTime);
        builder.HasIndex(p => p.WeekdayCloseTime);
        builder.HasIndex(p => p.SaturdayOpenTime);
        builder.HasIndex(p => p.SaturdayCloseTime);
        builder.HasIndex(p => p.SundayOpenTime);
        builder.HasIndex(p => p.SundayCloseTime);

        builder.Property(p => p.Type)
            .HasMaxLength(250);

        builder.Property(p => p.Location)
            .HasMaxLength(250);

        builder.Property(p => p.Area)
            .HasMaxLength(250);

        builder.Property(p => p.ExtraInformation)
            .HasMaxLength(500);

        builder.Property(p => p.DateAdded)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.WeekdayOpenTime)
            .HasColumnType("datetime2");

        builder.Property(p => p.WeekdayCloseTime)
            .HasColumnType("datetime2");

        builder.Property(p => p.SaturdayOpenTime)
            .HasColumnType("datetime2");

        builder.Property(p => p.SaturdayCloseTime)
            .HasColumnType("datetime2");

        builder.Property(p => p.SundayOpenTime)
            .HasColumnType("datetime2");

        builder.Property(p => p.SundayCloseTime)
            .HasColumnType("datetime2");

        builder.Property(p => p.Budget)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.RentPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.AvailableForRent)
            .HasColumnType("bit");

        builder.HasIndex(p => p.Area);
    }
}
