using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minimal.Infrastructure.Mapping;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("Roles");

        // PrimaryKey
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Id)
            .IsUnique();
    }
}
