using AwesomeAssertions;
using LanguageExt;
using LanguageExt.Common;
using SimpleCrm.Api.Features.Leads.CreateLead;

namespace SimpleCrm.Api.Tests.Features.Leads.CreateLead;

public class CreateLeadValidatorTests
{
    [Fact]
    public void Validate_WithValidRequest_ReturnsSuccess()
    {
        var request = new CreateLeadRequest("John Doe", "john@example.com", null, null);

        var result = CreateLeadValidator.Validate(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithAllFields_ReturnsSuccess()
    {
        var request = new CreateLeadRequest("Jane Smith", "jane@company.com", "+1234567890", "ACME Corp");

        var result = CreateLeadValidator.Validate(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyOrNullName_ReturnsNameRequiredError(string? name)
    {
        var request = new CreateLeadRequest(name, "john@example.com", null, null);

        var result = CreateLeadValidator.Validate(request);

        var errors = ExtractErrors(result);
        errors.Should().ContainSingle(e => e.Message == "Name is required");
    }

    [Fact]
    public void Validate_WithNameExceedingMaxLength_ReturnsError()
    {
        var longName = new string('a', 101);
        var request = new CreateLeadRequest(longName, "john@example.com", null, null);

        var result = CreateLeadValidator.Validate(request);

        var errors = ExtractErrors(result);
        errors.Should().ContainSingle(e => e.Message == "Name must not exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithEmptyOrNullEmail_ReturnsEmailRequiredError(string? email)
    {
        var request = new CreateLeadRequest("John Doe", email, null, null);

        var result = CreateLeadValidator.Validate(request);

        var errors = ExtractErrors(result);
        errors.Should().ContainSingle(e => e.Message == "Email is required");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing-domain@")]
    [InlineData("@missinglocal.com")]
    public void Validate_WithInvalidEmail_ReturnsError(string email)
    {
        var request = new CreateLeadRequest("John Doe", email, null, null);

        var result = CreateLeadValidator.Validate(request);

        var errors = ExtractErrors(result);
        errors.Should().ContainSingle(e => e.Message == "Email is not valid");
    }

    [Fact]
    public void Validate_WithMultipleInvalidFields_AccumulatesAllErrors()
    {
        var request = new CreateLeadRequest("", "notanemail", null, null);

        var result = CreateLeadValidator.Validate(request);

        var errors = ExtractErrors(result);
        errors.Should().HaveCount(2);
    }

    [Fact]
    public void Validate_WithEmailExceedingMaxLength_ReturnsError()
    {
        var longEmail = new string('a', 195) + "@b.com";
        var request = new CreateLeadRequest("John Doe", longEmail, null, null);

        var result = CreateLeadValidator.Validate(request);

        var errors = ExtractErrors(result);
        errors.Should().ContainSingle(e => e.Message == "Email must not exceed 200 characters");
    }

    private static Seq<Error> ExtractErrors<T>(Validation<Error, T> validation) =>
        validation.Match(
            Succ: _ => Seq<Error>.Empty,
            Fail: errors => errors
        );
}
