using LanguageExt;
using static LanguageExt.Prelude;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadValidator
{
    public static Validation<Seq<string>, CreateLeadRequest> Validate(CreateLeadRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("Email is required.");
        }
        else if (!request.Email.Contains('@'))
        {
            errors.Add("Email must be valid.");
        }

        return errors.Count > 0
            ? Fail<Seq<string>, CreateLeadRequest>(toSeq(errors))
            : Success<Seq<string>, CreateLeadRequest>(request);
    }
}
