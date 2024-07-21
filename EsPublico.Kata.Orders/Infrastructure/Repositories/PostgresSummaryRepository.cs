using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Databases;
using LanguageExt;

namespace EsPublico.Kata.Orders.Infrastructure.Repositories;

public class PostgresSummaryRepository(PostgresAdapter postgresAdapter) : SummaryRepository
{
    public async Task<Either<Error, List<SummaryRecord>>> GetSummary()
    {
        try
        {
            var summary = await postgresAdapter.Get<List<SummaryRecord>>(async (command) =>
            {
                command.CommandText = GetSummaryQuery();
                await using var reader = await command.ExecuteReaderAsync();

                var summaryRecords = new List<SummaryRecord>();
                while (await reader.ReadAsync())
                {
                    var category = reader.GetString(0);
                    var type = reader.GetString(1);
                    var count = reader.GetInt32(2);
                    summaryRecords.Add(new SummaryRecord(category, type, count));
                }

                return summaryRecords;
            });
            return summary;
        }
        catch (Exception e)
        {
            return new Error(e.Message);
        }
    }

    private static string GetSummaryQuery()
    {
        return @"
            SELECT 'Region' AS category, region AS type, COUNT(*) AS count
            FROM orders
            GROUP BY region

            UNION ALL

            SELECT 'Country' AS category, country AS type, COUNT(*) AS count
            FROM orders
            GROUP BY country

            UNION ALL

            SELECT 'Item Type' AS category, item_type AS type, COUNT(*) AS count
            FROM orders
            GROUP BY item_type

            UNION ALL

            SELECT 'Sales Channel' AS category, sales_channel AS type, COUNT(*) AS count
            FROM orders
            GROUP BY sales_channel

            UNION ALL

            SELECT 'Order Priority' AS category, order_priority AS type, COUNT(*) AS count
            FROM orders
            GROUP BY order_priority

            ORDER BY category, count DESC;";
    }
}