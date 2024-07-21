using EsPublico.Kata.Orders.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace EsPublico.Kata.Orders.Application;

public class SummaryService(
    SummaryRepository summaryRepository,
    FilesRepository filesRepository,
    ILogger<SummaryService> logger)
{
    public async Task Summary()
    {
        var maybeSummary = await summaryRepository.GetSummary();
        if (maybeSummary.IsLeft)
        {
            logger.LogError("Error getting summary: {Error}", maybeSummary.LeftAsEnumerable().First());
            throw new Exception("Error getting summary");
        }

        var summary = maybeSummary.RightAsEnumerable().First();
        var saveResult = await filesRepository.Save(summary);
        if (saveResult.IsLeft)
        {
            throw new Exception("Error saving summary");
        }
    }
}