using Microsoft.EntityFrameworkCore;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Data;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);
    }
}
