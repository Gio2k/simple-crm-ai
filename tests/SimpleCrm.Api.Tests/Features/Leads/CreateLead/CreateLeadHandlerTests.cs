using AwesomeAssertions;
using LanguageExt;
using SimpleCrm.Api.Features.Leads.CreateLead;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Tests.Features.Leads.CreateLead;

public class CreateLeadHandlerTests
{
    [Fact]
    public void Validate_WithValidRequest_ReturnsSuccess()
    {
        var request = new CreateLeadRequest("John Doe", "john@example.com", "555-1234", "Acme Corp");

        var result = CreateLeadHandler.Validate(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullName_ReturnsFailure()
    {
        var request = new CreateLeadRequest(null, "john@example.com", null, null);

        var result = CreateLeadHandler.Validate(request);

        result.IsFail.Should().BeTrue();
        var errors = result.FailToSeq();
        errors.Should().ContainSingle(e => e.Message == "Name is required.");
    }

    [Fact]
    public void Validate_WithEmptyName_ReturnsFailure()
    {
        var request = new CreateLeadRequest("   ", "john@example.com", null, null);

        var result = CreateLeadHandler.Validate(request);

        result.IsFail.Should().BeTrue();
        var errors = result.FailToSeq();
        errors.Should().ContainSingle(e => e.Message == "Name is required.");
    }

    [Fact]
    public void Validate_WithNullEmail_ReturnsFailure()
    {
        var request = new CreateLeadRequest("John Doe", null, null, null);

        var result = CreateLeadHandler.Validate(request);

        result.IsFail.Should().BeTrue();
        var errors = result.FailToSeq();
        errors.Should().ContainSingle(e => e.Message == "Email is required.");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ReturnsFailure()
    {
        var request = new CreateLeadRequest("John Doe", "not-an-email", null, null);

        var result = CreateLeadHandler.Validate(request);

        result.IsFail.Should().BeTrue();
        var errors = result.FailToSeq();
        errors.Should().ContainSingle(e => e.Message == "Email must be a valid email address.");
    }

    [Fact]
    public void Validate_WithNullNameAndNullEmail_ReturnsBothErrors()
    {
        var request = new CreateLeadRequest(null, null, null, null);

        var result = CreateLeadHandler.Validate(request);

        result.IsFail.Should().BeTrue();
        var errors = result.FailToSeq();
        errors.Count.Should().Be(2);
        errors.Should().Contain(e => e.Message == "Name is required.");
        errors.Should().Contain(e => e.Message == "Email is required.");
    }

    [Fact]
    public void Validate_TrimsNameAndEmail()
    {
        var request = new CreateLeadRequest("  John Doe  ", "  john@example.com  ", null, null);

        var result = CreateLeadHandler.Validate(request);

        result.IsSuccess.Should().BeTrue();
        result.IfSuccess(r =>
        {
            r.Name.Should().Be("John Doe");
            r.Email.Should().Be("john@example.com");
        });
    }

    [Fact]
    public void CreateLead_SetsCorrectProperties()
    {
        var request = new CreateLeadRequest("John Doe", "john@example.com", "555-1234", "Acme Corp");

        var lead = CreateLeadHandler.CreateLead(request);

        lead.Name.Should().Be("John Doe");
        lead.Email.Should().Be("john@example.com");
        lead.Phone.Should().Be("555-1234");
        lead.Company.Should().Be("Acme Corp");
        lead.Status.Should().Be(LeadStatus.New);
        lead.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateLead_WithNullOptionalFields_Succeeds()
    {
        var request = new CreateLeadRequest("John Doe", "john@example.com", null, null);

        var lead = CreateLeadHandler.CreateLead(request);

        lead.Name.Should().Be("John Doe");
        lead.Email.Should().Be("john@example.com");
        lead.Phone.Should().BeNull();
        lead.Company.Should().BeNull();
    }
}
