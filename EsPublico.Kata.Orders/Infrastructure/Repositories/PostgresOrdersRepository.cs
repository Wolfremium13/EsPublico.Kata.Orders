using System.Text;
using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using EsPublico.Kata.Orders.Infrastructure.Databases;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EsPublico.Kata.Orders.Infrastructure.Repositories;

public class PostgresOrdersRepository(
    PostgresAdapter adapter,
    ILogger<PostgresOrdersRepository> logger) : OrdersRepository
{
    public async Task<Either<Error, Unit>> Save(List<Order> orders)
    {
        try
        {
            var (sql, parameters) = BuildSaveOrdersQuery(orders);
            await adapter.Execute(async cmd =>
            {
                cmd.CommandText = sql;
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                await cmd.ExecuteNonQueryAsync();
            });
            logger.LogInformation($"Saved {orders.Count} orders");
            return Unit.Default;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving orders");
            return new Error(ex.Message);
        }
    }

    private static (string sql, List<NpgsqlParameter> parameters) BuildSaveOrdersQuery(List<Order> orders)
    {
        var sql = new StringBuilder();
        var parameters = new List<NpgsqlParameter>();

        sql.Append(
            "INSERT INTO orders (uuid, id, region, country, item_type, sales_channel, priority, date, ship_date, units_sold, unit_price, unit_cost, total_revenue, total_cost, total_profit) VALUES ");

        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            if (i > 0) sql.Append(", ");

            sql.Append(
                $"(@uuid{i}, @id{i}, @region{i}, @country{i}, @itemType{i}, @salesChannel{i}, @priority{i}, @date{i}, @shipDate{i}, @unitsSold{i}, @unitPrice{i}, @unitCost{i}, @totalRevenue{i}, @totalCost{i}, @totalProfit{i})");

            parameters.Add(new NpgsqlParameter($"@uuid{i}", order.Uuid.ToString()));
            parameters.Add(new NpgsqlParameter($"@id{i}", order.Id.Value));
            parameters.Add(new NpgsqlParameter($"@region{i}", order.Region.ToString()));
            parameters.Add(new NpgsqlParameter($"@country{i}", order.Country.ToString()));
            parameters.Add(new NpgsqlParameter($"@itemType{i}", order.ItemType.ToString()));
            parameters.Add(new NpgsqlParameter($"@salesChannel{i}", order.SalesChannel.ToString()));
            parameters.Add(new NpgsqlParameter($"@priority{i}", order.Priority.ToString()));
            parameters.Add(new NpgsqlParameter($"@date{i}", order.Date.ToString()));
            parameters.Add(new NpgsqlParameter($"@shipDate{i}", order.ShipDate.ToString()));
            parameters.Add(new NpgsqlParameter($"@unitsSold{i}", order.UnitsSold.Value));
            parameters.Add(new NpgsqlParameter($"@unitPrice{i}", order.UnitPrice.Value));
            parameters.Add(new NpgsqlParameter($"@unitCost{i}", order.UnitCost.Value));
            parameters.Add(new NpgsqlParameter($"@totalRevenue{i}", order.TotalRevenue.Value));
            parameters.Add(new NpgsqlParameter($"@totalCost{i}", order.TotalCost.Value));
            parameters.Add(new NpgsqlParameter($"@totalProfit{i}", order.TotalProfit.Value));
        }

        return (sql.ToString(), parameters);
    }
}