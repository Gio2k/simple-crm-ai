using AwesomeAssertions;

using SimpleCrm.Api.Data.Leads;
using SimpleCrm.Api.Features.Leads.CreateLead;
using SimpleCrm.Api.Features.Leads.Domain;

namespace SimpleCrm.Api.Tests.Features.Leads.CreateLead;

public class CreateLeadHandlerTests
{
    [Fact]
    public void CreateLead_BuildsNewLeadWithNewStatusAndFreshTimestamp()
    {
        var request = new ValidatedCreateLeadRequest("Alice Smith", "alice@example.com", "555-1234", "Acme Corp");

        var before = DateTime.UtcNow;
        var lead = CreateLeadHandler.CreateLead(request);
        var after = DateTime.UtcNow;

        lead.Id.Should().NotBe(Guid.Empty);
        lead.Name.Should().Be("Alice Smith");
        lead.Email.Should().Be("alice@example.com");
        lead.Phone.Should().Be("555-1234");
        lead.Company.Should().Be("Acme Corp");
        lead.Status.Should().Be(LeadStatus.New);
        lead.CreatedAt.Should().BeOnOrAfter(before);
        lead.CreatedAt.Should().BeOnOrBefore(after);
        lead.UpdatedAt.Should().BeNull();
        lead.FollowUps.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_PersistsLeadAndReturnsRight()
    {
        var request = new ValidatedCreateLeadRequest("Alice Smith", "alice@example.com", null, null);
        var repository = new FakeLeadRepository();

        var result = await CreateLeadHandler.HandleAsync(request, repository, CancellationToken.None);

        var lead = result.Match(
            Left: error => throw new Xunit.Sdk.XunitException($"Expected Right but got Left: {error.Message}"),
            Right: l => l);

        lead.Should().NotBeNull();
        repository.Stored.Should().BeSameAs(lead);
        lead.Status.Should().Be(LeadStatus.New);
    }

    [Fact]
    public async Task HandleAsync_WhenStoreFails_ReturnsLeftError()
    {
        var request = new ValidatedCreateLeadRequest("Alice Smith", "alice@example.com", null, null);
        var repository = new FakeLeadRepository
        {
            ExceptionToThrow = new InvalidOperationException("database unavailable"),
        };

        var result = await CreateLeadHandler.HandleAsync(request, repository, CancellationToken.None);

        result.IsLeft.Should().BeTrue();
        var error = result.Match(Left: e => e, Right: _ => throw new Xunit.Sdk.XunitException("Expected Left"));
        error.Message.Should().NotBeNullOrEmpty();
    }

    private sealed class FakeLeadRepository : ILeadRepository
    {
        public Lead? Stored { get; private set; }

        public Exception? ExceptionToThrow { get; set; }

        public Task StoreAsync(Lead lead, CancellationToken ct)
        {
            Stored = lead;
            return ExceptionToThrow is null
                ? Task.CompletedTask
                : Task.FromException(ExceptionToThrow);
        }
    }
}