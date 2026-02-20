using Marten;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadEndpoint
{
    public static IEndpointRouteBuilder MapCreateLead(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/leads", async (
            CreateLeadRequest request,
            IDocumentSession session,
            CancellationToken cancellationToken) =>
        {
            return await CreateLeadValidator.Validate(request).Match(
                Succ: async validRequest =>
                {
                    var result = await CreateLeadHandler.HandleAsync(validRequest, session, cancellationToken);
                    return result.Match(
                        Right: lead => Results.Created($"/leads/{lead.Id}", lead),
                        Left: error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError)
                    );
                },
                Fail: errors => Task.FromResult<IResult>(
                    Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            [""] = [.. errors.Select(e => e.Message)]
                        }
                    )
                )
            );
        })
        .WithName("CreateLead");

        return routes;
    }
}
