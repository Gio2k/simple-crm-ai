namespace SimpleCrm.Api.Features.Leads.Domain;

public record FollowUp
{
    public Guid Id { get; init; }
    public Guid LeadId { get; init; }
    public required string Notes { get; init; }
    public FollowUpType Type { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? ScheduledFor { get; init; }
    public bool IsCompleted { get; init; }
}

public enum FollowUpType
{
    Call,
    Email,
    Meeting,
    Task,
    Note
}
