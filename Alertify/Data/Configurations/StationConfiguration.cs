using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alertify.Data.Configurations
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.ToTable("Station");

            builder.HasKey(s => s.StationID);
            builder.Property(s => s.StationID)
                   .HasColumnName("StationID")
                   .ValueGeneratedOnAdd();

            builder.Property(s => s.Name)
                   .HasColumnName("Name")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.HasIndex(s => s.Name)
                   .IsUnique();

            builder.Property(s => s.Latitude)
                   .HasColumnName("Latitude")
                   .HasColumnType("decimal(10, 8)")
                   .IsRequired();

            builder.Property(s => s.Longitude)
                   .HasColumnName("Longitude")
                   .HasColumnType("decimal(11, 8)")
                   .IsRequired();

            builder.Property(s => s.ServiceType)
                   .HasColumnName("ServiceType")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(s => s.Address)
                   .HasColumnName("Address")
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(s => s.Phone)
                   .HasColumnName("Phone")
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(s => s.CreatedBy)
                   .HasColumnName("CreatedBy")
                   .IsRequired();

            builder.Property(s => s.CreationDate)
                   .HasColumnName("CreationDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(s => s.ModifiedBy)
                   .HasColumnName("ModifiedBy")
                   .IsRequired(false);

            builder.Property(s => s.ModificationDate)
                   .HasColumnName("ModificationDate")
                   .IsRequired(false);

            builder.Property(s => s.Status)
                   .HasColumnName("Status")
                   .HasMaxLength(20)
                   .HasDefaultValue("Active")
                   .IsRequired();

            builder.HasMany(s => s.Units)
                   .WithOne(u => u.Station)
                   .HasForeignKey(u => u.StationID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}