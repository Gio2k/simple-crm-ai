using LanguageExt;

using AwesomeAssertions;

using SimpleCrm.Api.Features.Leads.CreateLead;

namespace SimpleCrm.Api.Tests.Features.Leads.CreateLead;

public class CreateLeadValidatorTests
{
    [Fact]
    public void Validate_WithValidRequest_ReturnsSuccessWithTrimmedValues()
    {
        var request = new CreateLeadRequest
        {
            Name = "  Alice Smith  ",
            Email = "alice@example.com",
            Phone = " 555-1234 ",
            Company = " Acme Corp ",
        };

        var result = CreateLeadValidator.Validate(request);

        result.IsSuccess.Should().BeTrue();
        var validated = SuccessOf(result);
        validated.Name.Should().Be("Alice Smith");
        validated.Email.Should().Be("alice@example.com");
        validated.Phone.Should().Be("555-1234");
        validated.Company.Should().Be("Acme Corp");
    }

    [Fact]
    public void Validate_WithAllOptionalFieldsBlank_TreatsThemAsNull()
    {
        var request = new CreateLeadRequest
        {
            Name = "Alice Smith",
            Email = "alice@example.com",
            Phone = "   ",
            Company = "\t",
        };

        var result = CreateLeadValidator.Validate(request);

        result.IsSuccess.Should().BeTrue();
        var validated = SuccessOf(result);
        validated.Phone.Should().BeNull();
        validated.Company.Should().BeNull();
    }

    [Fact]
    public void Validate_WithMissingName_ReturnsRequirementError()
    {
        var request = new CreateLeadRequest { Name = null, Email = "alice@example.com" };

        var result = CreateLeadValidator.Validate(request);

        result.IsFail.Should().BeTrue();
        GetErrors(result).Should().ContainSingle(e => e.Property == "Name" && e.Message == "Name is required.");
    }

    [Fact]
    public void Validate_WithMissingNameAndEmail_AccumulatesBothErrors()
    {
        var request = new CreateLeadRequest { Name = " ", Email = "" };

        var errors = CreateLeadValidator.Validate(request).FailAsEnumerable().ToList();

        errors.Should().HaveCount(2);
        errors.Should().Contain(e => e.Property == "Name" && e.Message == "Name is required.");
        errors.Should().Contain(e => e.Property == "Email" && e.Message == "Email is required.");
    }

    [Fact]
    public void Validate_WithInvalidEmailFormat_ReturnsFormatError()
    {
        var request = new CreateLeadRequest { Name = "Alice", Email = "not-an-email" };

        var result = CreateLeadValidator.Validate(request);

        result.IsFail.Should().BeTrue();
        GetErrors(result).Should().ContainSingle(e => e.Property == "Email" && e.Message == "Email is not in a valid format.");
    }

    [Fact]
    public void Validate_WithNameExceedingMaxLength_ReturnsLengthError()
    {
        var request = new CreateLeadRequest { Name = new string('a', 101), Email = "alice@example.com" };

        var result = CreateLeadValidator.Validate(request);

        result.IsFail.Should().BeTrue();
        GetErrors(result).Should().Contain(e => e.Property == "Name" && e.Message.Contains("characters or fewer"));
    }

    [Fact]
    public void Validate_WithPhoneExceedingMaxLength_ReturnsLengthError()
    {
        var request = new CreateLeadRequest
        {
            Name = "Alice",
            Email = "alice@example.com",
            Phone = new string('1', 51),
        };

        var result = CreateLeadValidator.Validate(request);

        result.IsFail.Should().BeTrue();
        GetErrors(result).Should().ContainSingle(e => e.Property == "Phone");
    }

    [Fact]
    public void Validate_WithCompanyExceedingMaxLength_ReturnsLengthError()
    {
        var request = new CreateLeadRequest
        {
            Name = "Alice",
            Email = "alice@example.com",
            Company = new string('c', 151),
        };

        var result = CreateLeadValidator.Validate(request);

        result.IsFail.Should().BeTrue();
        GetErrors(result).Should().ContainSingle(e => e.Property == "Company");
    }

    private static IEnumerable<CreateLeadValidationError> GetErrors(
        Validation<CreateLeadValidationError, ValidatedCreateLeadRequest> result) =>
        result.Match(
            Succ: _ => Array.Empty<CreateLeadValidationError>(),
            Fail: errors => errors.AsEnumerable());

    private static ValidatedCreateLeadRequest SuccessOf(
        Validation<CreateLeadValidationError, ValidatedCreateLeadRequest> result) =>
        result.Match(
            Succ: v => v,
            Fail: errors => throw new Xunit.Sdk.XunitException(
                $"Expected success but got: {string.Join(", ", errors.Select(e => e.Message))}"));
}