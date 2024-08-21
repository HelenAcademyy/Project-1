using Helen.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserDataConfiguration : IEntityTypeConfiguration<UserData>
{
    public void Configure(EntityTypeBuilder<UserData> builder)
    {

            builder.HasKey(u => u.Username);


            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd() // This ensures the Id is auto-incremented
                .IsRequired();


            builder.Property(u => u.Location)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);


            builder.Property(u => u.Budget)
                .HasColumnType("decimal(18,2)");

            builder.Property(u => u.SendViaMail)
                .IsRequired();

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.HasIndex(u => u.Location);
            // Ensure Id is auto-incremented for UserData if it exists
            if (builder.Metadata.FindProperty("Id") != null)
            {
                builder.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();
            }
    }
}