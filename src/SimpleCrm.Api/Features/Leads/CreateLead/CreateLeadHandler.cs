using LanguageExt;

using static LanguageExt.Prelude;

using SimpleCrm.Api.Data.Leads;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

/// <summary>Failure while persisting a new lead.</summary>
public sealed record CreateLeadError(string Message);

/// <summary>
/// Create Lead use case — functional core, imperative shell.
/// Pure domain logic builds the new <see cref="Lead"/>; the repository call is
/// the side effect, isolated at the boundary and wrapped in <see cref="Either{L,R}"/>.
/// </summary>
public static class CreateLeadHandler
{
    /// <summary>Pure function: constructs a new lead from validated input.</summary>
    public static Lead CreateLead(ValidatedCreateLeadRequest request) => new()
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Email = request.Email,
        Phone = request.Phone,
        Company = request.Company,
        Status = LeadStatus.New,
        CreatedAt = DateTime.UtcNow,
    };

    /// <summary>Persists the lead. Returns <c>Left</c> when persistence fails.</summary>
    public static async Task<Either<CreateLeadError, Lead>> HandleAsync(
        ValidatedCreateLeadRequest request,
        ILeadRepository repository,
        CancellationToken ct)
    {
        var lead = CreateLead(request);

        try
        {
            await repository.StoreAsync(lead, ct);
        }
        catch
        {
            return Left<CreateLeadError, Lead>(new CreateLeadError("The lead could not be saved. Please try again."));
        }

        return Right<CreateLeadError, Lead>(lead);
    }
}