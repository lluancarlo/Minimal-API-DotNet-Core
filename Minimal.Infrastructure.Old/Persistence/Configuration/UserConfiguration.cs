using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.DAL.Entities;

namespace Minimal.Infrastructure.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // PrimaryKey
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id);
        builder.HasIndex(p => p.Id)
            .IsUnique();

        // ForeignKey
        builder.HasOne(p => p.Country)
            .WithMany()
            .HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.NoAction);

        // Columns
        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(254);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.CountryId)
            .IsRequired();
        builder.Property(p => p.Level)
            .IsRequired()
            .HasDefaultValue(1);
        builder.Property(p => p.Experience)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(p => p.Gold)
            .IsRequired()
            .HasDefaultValue(0);
    }
}