using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alertify.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");

            builder.HasKey(n => n.NotificationID);
            builder.Property(n => n.NotificationID)
                   .HasColumnName("NotificationID")
                   .ValueGeneratedOnAdd();

            builder.Property(n => n.UserID)
                   .HasColumnName("UserID")
                   .IsRequired(false);

            builder.Property(n => n.RecipientEmail)
                   .HasColumnName("RecipientEmail")
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(n => n.RecipientPhone)
                   .HasColumnName("RecipientPhone")
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(n => n.EmergencyID)
                   .HasColumnName("EmergencyID")
                   .IsRequired(false);

            builder.Property(n => n.EmergencyAssignmentID)
                   .HasColumnName("EmergencyAssignmentID")
                   .IsRequired(false);

            builder.Property(n => n.NotificationType)
                   .HasColumnName("NotificationType")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(n => n.SendChannel)
                   .HasColumnName("SendChannel")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(n => n.Subject)
                   .HasColumnName("Subject")
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(n => n.Message)
                   .HasColumnName("Message")
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired(false);

            builder.Property(n => n.SendDate)
                   .HasColumnName("SendDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(n => n.SendStatus)
                   .HasColumnName("SendStatus")
                   .HasMaxLength(50)
                   .HasDefaultValue("Sent")
                   .IsRequired();

            builder.Property(n => n.ErrorMessage)
                   .HasColumnName("ErrorMessage")
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired(false);

            builder.Property(n => n.ReadDate)
                   .HasColumnName("ReadDate")
                   .IsRequired(false);

            builder.Property(n => n.IsRead)
                   .HasColumnName("IsRead")
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(n => n.CreatedBy)
                   .HasColumnName("CreatedBy")
                   .IsRequired();

            builder.Property(n => n.CreationDate)
                   .HasColumnName("CreationDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(n => n.ModifiedBy)
                   .HasColumnName("ModifiedBy")
                   .IsRequired(false);

            builder.Property(n => n.ModificationDate)
                   .HasColumnName("ModificationDate")
                   .IsRequired(false);

            builder.Property(n => n.Status)
                   .HasColumnName("Status")
                   .HasMaxLength(20)
                   .HasDefaultValue("Active")
                   .IsRequired();

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Emergency)
                   .WithMany(e => e.Notifications)
                   .HasForeignKey(n => n.EmergencyID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.EmergencyAssignment)
                   .WithMany(ea => ea.Notifications)
                   .HasForeignKey(n => n.EmergencyAssignmentID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}