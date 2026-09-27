namespace SimpleCrm.Api.Features.Leads.CreateLead;

/// <summary>
/// Inbound DTO for creating a lead.
/// Properties are nullable on purpose: the validator is the single source of
/// truth for requiredness and format rules, producing user-friendly, accumulated
/// error messages (see <see cref="CreateLeadValidator"/>).
/// </summary>
public sealed record CreateLeadRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Company { get; init; }
}

/// <summary>
/// Immutable, post-validation request passed to the handler.
/// Guarantees (by construction via <see cref="CreateLeadValidator"/>):
/// non-empty trimmed Name/Email, and null-or-trimmed optional fields.
/// </summary>
public sealed record ValidatedCreateLeadRequest(string Name, string Email, string? Phone, string? Company);