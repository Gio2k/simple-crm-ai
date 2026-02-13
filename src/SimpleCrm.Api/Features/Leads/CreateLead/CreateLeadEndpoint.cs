using Marten;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadEndpoint
{
    public static IEndpointRouteBuilder MapCreateLeadEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/leads", async (CreateLeadRequest request, IDocumentSession session, CancellationToken cancellationToken) =>
        {
            var result = CreateLeadHandler.Handle(request);

            return await result.Match(
                Right: async lead =>
                {
                    session.Store(lead);
                    await session.SaveChangesAsync(cancellationToken);
                    return Results.Created($"/api/leads/{lead.Id}", lead);
                },
                Left: errors => Task.FromResult<IResult>(
                    Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["request"] = errors.ToArray()
                    }))
            );
        });

        return endpoints;
    }
}
