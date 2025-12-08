using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alertify.Data.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Unit");

            builder.HasKey(u => u.UnitID);
            builder.Property(u => u.UnitID)
                   .HasColumnName("UnitID")
                   .ValueGeneratedOnAdd();

            builder.Property(u => u.StationID)
                   .HasColumnName("StationID")
                   .IsRequired();

            builder.Property(u => u.Code)
                   .HasColumnName("Code")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasIndex(u => u.Code)
                   .IsUnique();

            builder.Property(u => u.Name)
                   .HasColumnName("Name")
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(u => u.ServiceType)
                   .HasColumnName("ServiceType")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(u => u.UnitStatus)
                   .HasColumnName("UnitStatus")
                   .HasMaxLength(50)
                   .HasDefaultValue("Available")
                   .IsRequired();

            builder.Property(u => u.ResponsiblePerson)
                   .HasColumnName("ResponsiblePerson")
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(u => u.ContactEmail)
                   .HasColumnName("ContactEmail")
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(u => u.ContactPhone)
                   .HasColumnName("ContactPhone")
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(u => u.CreatedBy)
                   .HasColumnName("CreatedBy")
                   .IsRequired();

            builder.Property(u => u.CreationDate)
                   .HasColumnName("CreationDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(u => u.ModifiedBy)
                   .HasColumnName("ModifiedBy")
                   .IsRequired(false);

            builder.Property(u => u.ModificationDate)
                   .HasColumnName("ModificationDate")
                   .IsRequired(false);

            builder.Property(u => u.Status)
                   .HasColumnName("Status")
                   .HasMaxLength(20)
                   .HasDefaultValue("Active")
                   .IsRequired();

            builder.HasOne(u => u.Station)
                   .WithMany(s => s.Units)
                   .HasForeignKey(u => u.StationID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.EmergencyAssignments)
                   .WithOne(ea => ea.Unit)
                   .HasForeignKey(ea => ea.UnitID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}