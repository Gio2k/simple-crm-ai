using AwesomeAssertions;
using SimpleCrm.Api.Features.Leads.CreateLead;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Tests;

public class CreateLeadTests
{
    [Fact]
    public void Validate_Should_Fail_When_Request_Is_Invalid()
    {
        var request = new CreateLeadRequest
        {
            Name = "",
            Email = "invalid-email"
        };

        var result = CreateLeadValidator.Validate(request);
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Handle_Should_Create_New_Lead_When_Request_Is_Valid()
    {
        var now = new DateTime(2026, 2, 1, 12, 0, 0, DateTimeKind.Utc);
        var request = new CreateLeadRequest
        {
            Name = "  Jane Doe  ",
            Email = "  jane@contoso.com  ",
            Phone = " 123 ",
            Company = " Contoso "
        };

        var result = CreateLeadHandler.Handle(request, () => now);

        result.Match(
            Right: lead =>
            {
                lead.Id.Should().NotBe(Guid.Empty);
                lead.Name.Should().Be("Jane Doe");
                lead.Email.Should().Be("jane@contoso.com");
                lead.Phone.Should().Be("123");
                lead.Company.Should().Be("Contoso");
                lead.Status.Should().Be(LeadStatus.New);
                lead.CreatedAt.Should().Be(now);
            },
            Left: _ => throw new Xunit.Sdk.XunitException("Expected successful lead creation."));
    }
}
