using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Alertify.Data
{
    public class AlertifyDbContext : DbContext
    {
        public AlertifyDbContext(DbContextOptions<AlertifyDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }
        public DbSet<EmergencyAssignment> EmergencyAssignments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmergencyStatusHistory> EmergencyStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}