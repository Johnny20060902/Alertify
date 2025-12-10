using Alertify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alertify.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            builder.HasKey(u => u.UserID);
            builder.Property(u => u.UserID)
                   .HasColumnName("UserID")
                   .ValueGeneratedOnAdd();

            builder.Property(u => u.Email)
                   .HasColumnName("Email")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.Password)
                   .HasColumnName("Password")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(u => u.FirstName)
                   .HasColumnName("FirstName")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.FirstLastName)
                   .HasColumnName("FirstLastName")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.SecondLastName)
                   .HasColumnName("SecondLastName")
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(u => u.NationalID)
                   .HasColumnName("NationalID")
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(u => u.Phone)
                   .HasColumnName("Phone")
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(u => u.Role)
                   .HasColumnName("Role")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(u => u.MustChangePassword)
                    .HasColumnName("MustChangePassword")
                    .IsRequired()
                    .HasDefaultValue(false);

            builder.Property(u => u.LastAccess)
                   .HasColumnName("LastAccess")
                   .IsRequired(false);

            builder.Property(u => u.ProfilePhotoURL)
                   .HasColumnName("ProfilePhotoURL")
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(u => u.RegistrationDate)
                   .HasColumnName("RegistrationDate")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

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

            builder.HasMany(u => u.Emergencies)
                   .WithOne(e => e.Citizen)
                   .HasForeignKey(e => e.CitizenID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Notifications)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UserID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}