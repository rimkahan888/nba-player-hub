using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NbaHub.Domain.Teams;

namespace NbaHub.Infrastructure.Persistence.Configurations;

internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.City).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Abbreviation).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(t => t.Conference).HasConversion<string>().HasMaxLength(4).IsRequired();
        builder.Property(t => t.Division).HasMaxLength(20).IsRequired();
        builder.Property(t => t.PrimaryColour).HasMaxLength(7);

        builder.HasIndex(t => t.Abbreviation).IsUnique();
        builder.HasIndex(t => t.ExternalId).IsUnique();

        builder.Ignore(t => t.FullName);

        builder.HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(t => t.Players).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
