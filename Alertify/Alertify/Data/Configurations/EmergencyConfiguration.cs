using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alertify.Data.Configurations
{
    public class EmergencyConfiguration : IEntityTypeConfiguration<Emergency>
    {
        public void Configure(EntityTypeBuilder<Emergency> builder)
        {
            builder.ToTable("Emergency");

            builder.HasKey(e => e.EmergencyID);
            builder.Property(e => e.EmergencyID)
                   .HasColumnName("EmergencyID")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.CitizenID)
                   .HasColumnName("CitizenID")
                   .IsRequired();

            builder.Property(e => e.EmergencyCategory)
                   .HasColumnName("EmergencyCategory")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnName("Description")
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired(false);

            builder.Property(e => e.Latitude)
                   .HasColumnName("Latitude")
                   .HasColumnType("decimal(10, 8)")
                   .IsRequired();

            builder.Property(e => e.Longitude)
                   .HasColumnName("Longitude")
                   .HasColumnType("decimal(11, 8)")
                   .IsRequired();

            builder.Property(e => e.Address)
                   .HasColumnName("Address")
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(e => e.LocationReference)
                   .HasColumnName("LocationReference")
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(e => e.ImageURL)
                   .HasColumnName("ImageURL")
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(e => e.EmergencyStatus)
                   .HasColumnName("EmergencyStatus")
                   .HasMaxLength(50)
                   .HasDefaultValue("Pending")
                   .IsRequired();

            builder.Property(e => e.Priority)
                   .HasColumnName("Priority")
                   .HasMaxLength(20)
                   .HasDefaultValue("Medium")
                   .IsRequired();

            builder.Property(e => e.AssignmentDate)
                   .HasColumnName("AssignmentDate")
                   .IsRequired(false);

            builder.Property(e => e.ResolutionDate)
                   .HasColumnName("ResolutionDate")
                   .IsRequired(false);

            builder.Property(e => e.CreatedBy)
                   .HasColumnName("CreatedBy")
                   .IsRequired();

            builder.Property(e => e.CreationDate)
                   .HasColumnName("CreationDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(e => e.ModifiedBy)
                   .HasColumnName("ModifiedBy")
                   .IsRequired(false);

            builder.Property(e => e.ModificationDate)
                   .HasColumnName("ModificationDate")
                   .IsRequired(false);

            builder.Property(e => e.Status)
                   .HasColumnName("Status")
                   .HasMaxLength(20)
                   .HasDefaultValue("Active")
                   .IsRequired();

            builder.HasOne(e => e.Citizen)
                   .WithMany(u => u.Emergencies)
                   .HasForeignKey(e => e.CitizenID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.EmergencyAssignments)
                   .WithOne(ea => ea.Emergency)
                   .HasForeignKey(ea => ea.EmergencyID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Notifications)
                   .WithOne(n => n.Emergency)
                   .HasForeignKey(n => n.EmergencyID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.StatusHistory)
                   .WithOne(h => h.Emergency)
                   .HasForeignKey(h => h.EmergencyID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}