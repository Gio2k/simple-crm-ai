using LanguageExt;
using LanguageExt.Common;
using Marten;
using SimpleCrm.Api.Features.Leads.Domain;
using static LanguageExt.Prelude;

namespace SimpleCrm.Api.Features.Leads.CreateLead;

public static class CreateLeadHandler
{
    public static async Task<Either<Error, Lead>> HandleAsync(
        CreateLeadRequest request,
        IDocumentSession session,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                Name = request.Name!,
                Email = request.Email!,
                Phone = request.Phone,
                Company = request.Company,
                Status = LeadStatus.New,
                CreatedAt = DateTime.UtcNow
            };

            session.Store(lead);
            await session.SaveChangesAsync(cancellationToken);

            return Right<Error, Lead>(lead);
        }
        catch (Exception ex)
        {
            return Left<Error, Lead>(Error.New(ex));
        }
    }
}
