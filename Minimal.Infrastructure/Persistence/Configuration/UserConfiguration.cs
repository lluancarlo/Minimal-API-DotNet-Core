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

        // Columns
        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(254);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}