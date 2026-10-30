using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NbaHub.Domain.Players;

namespace NbaHub.Infrastructure.Persistence.Configurations;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Slug).HasMaxLength(100).IsRequired();
        builder.Property(p => p.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Position).HasMaxLength(10);
        builder.Property(p => p.JerseyNumber).HasMaxLength(3);
        builder.Property(p => p.College).HasMaxLength(100);
        builder.Property(p => p.Country).HasMaxLength(60);

        // Height is a value object in the domain and whole inches in the database.
        builder.Property(p => p.Height)
            .HasColumnName("HeightInches")
            .HasConversion(height => height!.Value.TotalInches, inches => Height.FromInches(inches));

        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.ExternalId).IsUnique();
        builder.HasIndex(p => p.TeamId);

        builder.Ignore(p => p.FullName);
    }
}
