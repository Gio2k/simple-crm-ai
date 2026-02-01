namespace SimpleCrm.Api.Features.Leads.Domain;

public record Lead
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public string? Company { get; init; }
    public LeadStatus Status { get; init; } = LeadStatus.New;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
    public ICollection<FollowUp> FollowUps { get; init; } = [];
}

public enum LeadStatus
{
    New,
    Contacted,
    Qualified,
    Proposal,
    Negotiation,
    Won,
    Lost
}
