using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alertify.Data.Configurations
{
    public class EmergencyAssignmentConfiguration : IEntityTypeConfiguration<EmergencyAssignment>
    {
        public void Configure(EntityTypeBuilder<EmergencyAssignment> builder)
        {
            builder.ToTable("EmergencyAssignment");

            builder.HasKey(ea => ea.EmergencyAssignmentID);
            builder.Property(ea => ea.EmergencyAssignmentID)
                   .HasColumnName("EmergencyAssignmentID")
                   .ValueGeneratedOnAdd();

            builder.Property(ea => ea.EmergencyID)
                   .HasColumnName("EmergencyID")
                   .IsRequired();

            builder.Property(ea => ea.UnitID)
                   .HasColumnName("UnitID")
                   .IsRequired();

            builder.Property(ea => ea.AssignedBy)
                   .HasColumnName("AssignedBy")
                   .IsRequired();

            builder.Property(ea => ea.AssignmentDate)
                   .HasColumnName("AssignmentDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(ea => ea.StartDate)
                   .HasColumnName("StartDate")
                   .IsRequired(false);

            builder.Property(ea => ea.CompletionDate)
                   .HasColumnName("CompletionDate")
                   .IsRequired(false);

            builder.Property(ea => ea.CalculatedDistance)
                   .HasColumnName("CalculatedDistance")
                   .HasColumnType("decimal(10, 2)")
                   .IsRequired(false);

            builder.Property(ea => ea.AssignmentStatus)
                   .HasColumnName("AssignmentStatus")
                   .HasMaxLength(50)
                   .HasDefaultValue("Assigned")
                   .IsRequired();

            builder.Property(ea => ea.Notes)
                   .HasColumnName("Notes")
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired(false);

            builder.Property(ea => ea.CreatedBy)
                   .HasColumnName("CreatedBy")
                   .IsRequired();

            builder.Property(ea => ea.CreationDate)
                   .HasColumnName("CreationDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(ea => ea.ModifiedBy)
                   .HasColumnName("ModifiedBy")
                   .IsRequired(false);

            builder.Property(ea => ea.ModificationDate)
                   .HasColumnName("ModificationDate")
                   .IsRequired(false);

            builder.Property(ea => ea.Status)
                   .HasColumnName("Status")
                   .HasMaxLength(20)
                   .HasDefaultValue("Active")
                   .IsRequired();

            builder.HasOne(ea => ea.Emergency)
                   .WithMany(e => e.EmergencyAssignments)
                   .HasForeignKey(ea => ea.EmergencyID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ea => ea.Unit)
                   .WithMany(u => u.EmergencyAssignments)
                   .HasForeignKey(ea => ea.UnitID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ea => ea.Notifications)
                   .WithOne(n => n.EmergencyAssignment)
                   .HasForeignKey(n => n.EmergencyAssignmentID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}