using LanguageExt;
using SimpleCrm.Api.Features.Leads.Domain;
using static LanguageExt.Prelude;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadHandler
{
    public static Either<Seq<string>, Lead> Handle(CreateLeadRequest request, Func<DateTime>? utcNow = null)
    {
        var now = (utcNow ?? (() => DateTime.UtcNow))();

        return CreateLeadValidator
            .Validate(request)
            .ToEither()
            .MapLeft(errors => errors.Bind(identity))
            .Map(validRequest => new Lead
            {
                Id = Guid.NewGuid(),
                Name = validRequest.Name.Trim(),
                Email = validRequest.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(validRequest.Phone) ? null : validRequest.Phone.Trim(),
                Company = string.IsNullOrWhiteSpace(validRequest.Company) ? null : validRequest.Company.Trim(),
                Status = LeadStatus.New,
                CreatedAt = now
            });
    }
}
