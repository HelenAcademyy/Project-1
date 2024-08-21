using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Helen.Repository.Models;

public class SmsDataConfiguration : IEntityTypeConfiguration<SmsData>
{
    public void Configure(EntityTypeBuilder<SmsData> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.From)
            .HasMaxLength(250);

        builder.Property(s => s.To)
            .HasMaxLength(250);

        builder.Property(s => s.Body)
            .HasMaxLength(3000); // Adjust based on SMS length limits

        builder.Property(s => s.Gateway)
            .HasMaxLength(50);

        builder.Property(e => e.SentDate)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(s => s.CallbackUrl)
            .HasMaxLength(500);

        builder.Property(s => s.CustomerReference)
            .HasMaxLength(250);

        builder.Property(s => s.Message)
            .HasMaxLength(250);

        builder.Property(s => s.Status)
            .HasMaxLength(20);

        builder.Property(s => s.Message_id)
            .HasMaxLength(50);

        builder.Property(s => s.Currency)
            .HasMaxLength(5);
    }
}
