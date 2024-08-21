using Microsoft.EntityFrameworkCore;
using Helen.Repository.Models;
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
        public DbSet<EmailData> EmailData { get; set; }
        public DbSet<SmsData> SmsData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LocationNotificationDataConfiguration());
            modelBuilder.ApplyConfiguration(new UserDataConfiguration());
            modelBuilder.ApplyConfiguration(new EmailDataConfiguration());
            modelBuilder.ApplyConfiguration(new SmsDataConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
