using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadValidator
{
    public static Validation<Error, CreateLeadRequest> Validate(CreateLeadRequest request) =>
        (ValidateName(request.Name), ValidateEmail(request.Email))
            .Apply((_, _) => request);

    private static Validation<Error, string> ValidateName(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? Fail<Error, string>(Error.New("Name is required"))
            : name.Trim().Length > 100
                ? Fail<Error, string>(Error.New("Name must not exceed 100 characters"))
                : Success<Error, string>(name.Trim());

    private static Validation<Error, string> ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Fail<Error, string>(Error.New("Email is required"));

        var trimmed = email.Trim();
        var atIndex = trimmed.IndexOf('@');

        if (atIndex <= 0 || atIndex == trimmed.Length - 1)
            return Fail<Error, string>(Error.New("Email is not valid"));

        if (trimmed.Length > 200)
            return Fail<Error, string>(Error.New("Email must not exceed 200 characters"));

        return Success<Error, string>(trimmed);
    }
}
