using AwesomeAssertions;
using Marten;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleCrm.Api.Features.Leads.CreateLead;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Tests.Features.Leads.CreateLead;

public class CreateLeadHandlerTests
{
    private readonly IDocumentSession _session = Substitute.For<IDocumentSession>();

    [Fact]
    public async Task HandleAsync_WithValidRequest_ReturnsCreatedLead()
    {
        var request = new CreateLeadRequest("John Doe", "john@example.com", "555-1234", "ACME Corp");

        var result = await CreateLeadHandler.HandleAsync(request, _session);

        var lead = result.Match(
            Right: l => l,
            Left: e => throw new Exception($"Expected success but got error: {e.Message}")
        );

        lead.Name.Should().Be("John Doe");
        lead.Email.Should().Be("john@example.com");
        lead.Phone.Should().Be("555-1234");
        lead.Company.Should().Be("ACME Corp");
        lead.Status.Should().Be(LeadStatus.New);
        lead.Id.Should().NotBe(Guid.Empty);
        lead.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _session.Received(1).Store(Arg.Any<Lead>());
        await _session.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithOptionalFieldsNull_ReturnsLeadWithNullOptionals()
    {
        var request = new CreateLeadRequest("Jane Smith", "jane@example.com", null, null);

        var result = await CreateLeadHandler.HandleAsync(request, _session);

        var lead = result.Match(Right: l => l, Left: e => throw new Exception(e.Message));

        lead.Phone.Should().BeNull();
        lead.Company.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_WhenSessionThrows_ReturnsLeft()
    {
        _session.SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));

        var request = new CreateLeadRequest("John Doe", "john@example.com", null, null);

        var result = await CreateLeadHandler.HandleAsync(request, _session);

        result.IsLeft.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenSessionThrows_ErrorMessageContainsExceptionMessage()
    {
        _session.SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Connection refused"));

        var request = new CreateLeadRequest("John Doe", "john@example.com", null, null);

        var result = await CreateLeadHandler.HandleAsync(request, _session);

        var error = result.Match(Right: _ => throw new Exception("Expected failure"), Left: e => e);
        error.Message.Should().Contain("Connection refused");
    }
}
