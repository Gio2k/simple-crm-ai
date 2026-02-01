using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Data;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(200);
        builder.Property(l => l.Email).IsRequired().HasMaxLength(200);
        builder.Property(l => l.Phone).HasMaxLength(50);
        builder.Property(l => l.Company).HasMaxLength(200);
        builder.Property(l => l.Status).HasConversion<string>();

        builder.HasMany(l => l.FollowUps)
            .WithOne()
            .HasForeignKey(f => f.LeadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FollowUpConfiguration : IEntityTypeConfiguration<FollowUp>
{
    public void Configure(EntityTypeBuilder<FollowUp> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Notes).IsRequired().HasMaxLength(2000);
        builder.Property(f => f.Type).HasConversion<string>();
    }
}
