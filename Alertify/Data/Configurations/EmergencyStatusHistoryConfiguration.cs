using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alertify.Data.Configurations
{
    public class EmergencyStatusHistoryConfiguration : IEntityTypeConfiguration<EmergencyStatusHistory>
    {
        public void Configure(EntityTypeBuilder<EmergencyStatusHistory> builder)
        {
            builder.ToTable("EmergencyStatusHistory");

            builder.HasKey(h => h.HistoryID);
            builder.Property(h => h.HistoryID)
                   .HasColumnName("HistoryID")
                   .ValueGeneratedOnAdd();

            builder.Property(h => h.EmergencyID)
                   .HasColumnName("EmergencyID")
                   .IsRequired();

            builder.Property(h => h.PreviousStatus)
                   .HasColumnName("PreviousStatus")
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(h => h.NewStatus)
                   .HasColumnName("NewStatus")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(h => h.ChangeDate)
                   .HasColumnName("ChangeDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(h => h.ChangedBy)
                   .HasColumnName("ChangedBy")
                   .IsRequired();

            builder.Property(h => h.Comment)
                   .HasColumnName("Comment")
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired(false);

            builder.HasOne(h => h.Emergency)
                   .WithMany(e => e.StatusHistory)
                   .HasForeignKey(h => h.EmergencyID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}