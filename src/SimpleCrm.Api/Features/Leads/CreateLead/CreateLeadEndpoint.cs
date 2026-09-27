using LanguageExt;

using SimpleCrm.Api.Data.Leads;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

/// <summary>
/// Minimal API endpoint for creating a lead.
/// Maps the functional pipeline (validation, use case) onto HTTP:
/// 201 with the created lead, 400 with accumulated validation errors, 500 on failure.
/// </summary>
public static class CreateLeadEndpoint
{
    public static IEndpointRouteBuilder MapCreateLeadEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/leads", HandleAsync);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        CreateLeadRequest? request,
        ILeadRepository repository,
        CancellationToken ct)
    {
        var validation = CreateLeadValidator.Validate(request ?? new CreateLeadRequest());

        return await validation.MatchAsync(
            validated => HandleValidatedAsync(validated, repository, ct),
            errors => Results.ValidationProblem(
                errors.AsEnumerable().ToDictionary(e => e.Property, e => new[] { e.Message })));
    }

    private static async Task<IResult> HandleValidatedAsync(
        ValidatedCreateLeadRequest validated,
        ILeadRepository repository,
        CancellationToken ct)
    {
        var result = await CreateLeadHandler.HandleAsync(validated, repository, ct);

        return await result.MatchAsync(
            lead => Task.FromResult<IResult>(Results.Created($"/api/leads/{lead.Id}", lead)),
            error => Results.Problem(
                detail: error.Message,
                title: "Lead could not be created",
                statusCode: StatusCodes.Status500InternalServerError));
    }
}