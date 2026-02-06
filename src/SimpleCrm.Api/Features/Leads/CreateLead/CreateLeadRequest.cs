namespace SimpleCrm.Api.Features.Leads.CreateLead;

public record CreateLeadRequest(
    string? Name,
    string? Email,
    string? Phone,
    string? Company);
