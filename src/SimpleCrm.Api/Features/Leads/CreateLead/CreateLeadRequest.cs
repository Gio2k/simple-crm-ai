namespace SimpleCrm.Api.Features.Leads.CreateLead;

public record CreateLeadRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Company { get; init; }
}
