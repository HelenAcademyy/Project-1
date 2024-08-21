using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Helen.Repository.Models;

public class EmailDataConfiguration : IEntityTypeConfiguration<EmailData>
{
    public void Configure(EntityTypeBuilder<EmailData> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.To)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(e => e.Body)
            .IsRequired();

        builder.Property(e => e.SentDate)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(e => e.ResponseMessage)
            .HasMaxLength(500);

        builder.Property(e => e.Message)
            .HasMaxLength(500);

        builder.Property(e => e.IsSuccessful)
            .IsRequired();
    }
}
