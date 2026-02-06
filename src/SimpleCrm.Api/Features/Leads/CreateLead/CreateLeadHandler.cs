using LanguageExt;
using LanguageExt.Common;
using Marten;
using SimpleCrm.Api.Features.Leads.Domain;
using static LanguageExt.Prelude;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadHandler
{
    public static Validation<Error, CreateLeadRequest> Validate(CreateLeadRequest request) =>
        (ValidateName(request.Name), ValidateEmail(request.Email))
            .Apply((name, email) => request with { Name = name, Email = email });

    public static Lead CreateLead(CreateLeadRequest request) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name!,
            Email = request.Email!,
            Phone = request.Phone,
            Company = request.Company,
            Status = LeadStatus.New,
            CreatedAt = DateTime.UtcNow
        };

    public static async Task<Either<Error, Lead>> Handle(
        CreateLeadRequest request,
        IDocumentSession session,
        CancellationToken ct)
    {
        return await Validate(request)
            .ToEither()
            .Map(CreateLead)
            .MapLeft(errors => Error.New(string.Join("; ", errors.Select(e => e.Message))))
            .MapAsync(async lead =>
            {
                session.Store(lead);
                await session.SaveChangesAsync(ct);
                return lead;
            });
    }

    private static Validation<Error, string> ValidateName(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? Fail<Error, string>(Error.New("Name is required."))
            : Success<Error, string>(name.Trim());

    private static Validation<Error, string> ValidateEmail(string? email) =>
        string.IsNullOrWhiteSpace(email)
            ? Fail<Error, string>(Error.New("Email is required."))
            : !email.Contains('@')
                ? Fail<Error, string>(Error.New("Email must be a valid email address."))
                : Success<Error, string>(email.Trim());
}
