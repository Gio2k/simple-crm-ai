using Marten;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Data.Leads;

/// <summary>
/// Marten-backed implementation of <see cref="ILeadRepository"/>.
/// Uses the scoped <see cref="IDocumentSession"/> so the unit of work
/// is aligned with the HTTP request.
/// </summary>
public sealed class MartenLeadRepository(IDocumentSession session) : ILeadRepository
{
    public async Task StoreAsync(Lead lead, CancellationToken ct)
    {
        session.Store(lead);
        await session.SaveChangesAsync(ct);
    }
}