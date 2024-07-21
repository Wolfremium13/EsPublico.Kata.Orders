using System.Text;
using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace EsPublico.Kata.Orders.Infrastructure.Repositories;

public class LocalFilesRepository(ILogger<LocalFilesRepository> logger) : FilesRepository
{
    public async Task<Either<Error, Unit>> Save(List<Order> orders, DateTime executionDate)
    {
        try
        {
            var folderDate = executionDate.ToString("yyyy-MM-ddTHH-mm-ss");
            var folderPath = Path.Combine(Path.GetTempPath(), $"kata-orders-{folderDate}");
            Directory.CreateDirectory(folderPath);

            var savingDate = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-fff");
            var filePath = Path.Combine(folderPath, $"orders_batch_{savingDate}.csv");

            var fileExists = File.Exists(filePath);
            await using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
            await using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                if (!fileExists)
                {
                    await writer.WriteLineAsync(
                        "uuid,id,region,country,item_type,sales_channel,priority,date,ship_date,units_sold,unit_price," +
                        "unit_cost,total_revenue,total_cost,total_profit");
                }

                foreach (var csvLine in orders.Select(order => string.Join(",",
                             order.Uuid.ToString(),
                             order.Id.Value,
                             order.Region.ToString(),
                             order.Country.ToString(),
                             order.ItemType.ToString(),
                             order.SalesChannel.ToString(),
                             order.Priority.ToString(),
                             order.Date.ToString(),
                             order.ShipDate.ToString(),
                             order.UnitsSold.Value,
                             order.UnitPrice.Value,
                             order.UnitCost.Value,
                             order.TotalRevenue.Value,
                             order.TotalCost.Value,
                             order.TotalProfit.Value)))
                {
                    await writer.WriteLineAsync(csvLine);
                }
            }

            logger.LogInformation($"Saved {orders.Count} orders to {filePath}");
            return Unit.Default;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving orders as CSV");
            return new Error(ex.Message);
        }
    }
}