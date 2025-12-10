using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace Alertify.Data
{
    public class AlertifyDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor actualizado
        public AlertifyDbContext(
            DbContextOptions<AlertifyDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }
        public DbSet<EmergencyAssignment> EmergencyAssignments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmergencyStatusHistory> EmergencyStatusHistories { get; set; }


        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseModel>();
            var currentUserId = GetCurrentUserId();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreationDate = DateTime.Now;
                    entry.Entity.CreatedBy = currentUserId ?? 1; 
                    entry.Entity.Status = "Activo";
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModificationDate = DateTime.Now;
                    entry.Entity.ModifiedBy = currentUserId;

                    entry.Property(nameof(BaseModel.CreationDate)).IsModified = false;
                    entry.Property(nameof(BaseModel.CreatedBy)).IsModified = false;
                }
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BaseModel>();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}