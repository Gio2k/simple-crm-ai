using System.Text.RegularExpressions;
using LanguageExt;

using static LanguageExt.Prelude;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

/// <summary>A single validation failure, keyed to the offending property.</summary>
public sealed record CreateLeadValidationError(string Property, string Message);

/// <summary>
/// Functional validation for <see cref="CreateLeadRequest"/> using language-ext
/// <see cref="Validation{CreateLeadValidationError,ValidatedCreateLeadRequest}"/>.
/// All rules run and their errors accumulate via the applicative chain.
/// </summary>
public static partial class CreateLeadValidator
{
    private const int MaxNameLength = 100;
    private const int MaxEmailLength = 254;
    private const int MaxPhoneLength = 50;
    private const int MaxCompanyLength = 150;

    public static Validation<CreateLeadValidationError, ValidatedCreateLeadRequest> Validate(CreateLeadRequest request) =>
        (ValidateName(request.Name), ValidateEmail(request.Email), ValidatePhone(request.Phone), ValidateCompany(request.Company))
            .Apply((name, email, phone, company) => new ValidatedCreateLeadRequest(
                name, email, EmptyToNull(phone), EmptyToNull(company)));

    private static Validation<CreateLeadValidationError, string> ValidateName(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? Fail<string>("Name", "Name is required.")
            : name.Trim().Length > MaxNameLength
                ? Fail<string>("Name", $"Name must be {MaxNameLength} characters or fewer.")
                : Success<CreateLeadValidationError, string>(name.Trim());

    private static Validation<CreateLeadValidationError, string> ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Fail<string>("Email", "Email is required.");

        email = email.Trim();

        if (email.Length > MaxEmailLength)
            return Fail<string>("Email", $"Email must be {MaxEmailLength} characters or fewer.");

        if (!EmailRegex().IsMatch(email))
            return Fail<string>("Email", "Email is not in a valid format.");

        return Success<CreateLeadValidationError, string>(email);
    }

    private static Validation<CreateLeadValidationError, string> ValidatePhone(string? phone) =>
        ValidateOptional(phone, MaxPhoneLength, "Phone", $"Phone must be {MaxPhoneLength} characters or fewer.");

    private static Validation<CreateLeadValidationError, string> ValidateCompany(string? company) =>
        ValidateOptional(company, MaxCompanyLength, "Company", $"Company must be {MaxCompanyLength} characters or fewer.");

    /// <summary>
    /// Optional fields: blank/whitespace is normalised to an empty-string sentinel
    /// (mapped to null by <see cref="EmptyToNull"/>); otherwise the value is trimmed
    /// and length-checked.
    /// </summary>
    private static Validation<CreateLeadValidationError, string> ValidateOptional(
        string? value, int maxLength, string property, string tooLongMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Success<CreateLeadValidationError, string>(string.Empty);

        value = value.Trim();

        return value.Length > maxLength
            ? Fail<string>(property, tooLongMessage)
            : Success<CreateLeadValidationError, string>(value);
    }

    private static string? EmptyToNull(string value) => value.Length == 0 ? null : value;

    private static Validation<CreateLeadValidationError, T> Fail<T>(string property, string message) =>
        LanguageExt.Prelude.Fail<CreateLeadValidationError, T>(new CreateLeadValidationError(property, message));

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();
}