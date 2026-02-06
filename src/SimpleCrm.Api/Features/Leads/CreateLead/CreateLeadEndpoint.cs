using Marten;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadEndpoint
{
    public static void MapCreateLeadEndpoint(this IEndpointRouteBuilder app) =>
        app.MapPost("/api/leads", async (
            CreateLeadRequest request,
            IDocumentSession session,
            CancellationToken ct) =>
        {
            var result = await CreateLeadHandler.Handle(request, session, ct);

            return result.Match(
                Right: lead => Results.Created($"/api/leads/{lead.Id}", lead),
                Left: error => Results.BadRequest(new { error = error.Message }));
        })
        .WithName("CreateLead")
        .WithTags("Leads")
        .Produces(StatusCodes.Status201Created)
        .ProducesValidationProblem();
}
