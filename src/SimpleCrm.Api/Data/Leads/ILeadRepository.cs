using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Data.Leads;

/// <summary>
/// Persistence port for Lead documents. The imperative shell (infrastructure)
/// lives behind this interface so the functional core stays testable.
/// </summary>
public interface ILeadRepository
{
    Task StoreAsync(Lead lead, CancellationToken ct);
}