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
            "INSERT INTO orders (uuid, order_id, region, country, item_type, sales_channel, order_priority, order_date, " +
            "ship_date, units_sold, unit_price, unit_cost, total_revenue, total_cost, total_profit) VALUES ");

        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            if (i > 0) sql.Append(", ");

            sql.Append(
                $"(@uuid{i}, @order_id{i}, @region{i}, @country{i}, @item_type{i}, @sales_channel{i}, " +
                $"@order_priority{i}, @order_date{i}, @ship_date{i}, @units_sold{i}, @unit_price{i}, @unit_cost{i}," +
                $" @total_revenue{i}, @total_cost{i}, @total_profit{i})");

            parameters.Add(new NpgsqlParameter($"@uuid{i}", NpgsqlTypes.NpgsqlDbType.Uuid)
                { Value = order.Uuid.ToGuid() });
            parameters.Add(new NpgsqlParameter($"@order_id{i}", order.Id.Value));
            parameters.Add(new NpgsqlParameter($"@region{i}", order.Region.ToString()));
            parameters.Add(new NpgsqlParameter($"@country{i}", order.Country.Value));
            parameters.Add(new NpgsqlParameter($"@item_type{i}", order.ItemType.ToString()));
            parameters.Add(new NpgsqlParameter($"@sales_channel{i}", order.SalesChannel.ToString()));
            parameters.Add(new NpgsqlParameter($"@order_priority{i}", order.Priority.ToString()));
            parameters.Add(new NpgsqlParameter($"@order_date{i}", NpgsqlTypes.NpgsqlDbType.Date)
                { Value = order.Date.ToDate() });
            parameters.Add(new NpgsqlParameter($"@ship_date{i}", NpgsqlTypes.NpgsqlDbType.Date)
                { Value = order.ShipDate.ToDate() });
            parameters.Add(new NpgsqlParameter($"@units_sold{i}", order.UnitsSold.Value));
            parameters.Add(new NpgsqlParameter($"@unit_price{i}", order.UnitPrice.Value));
            parameters.Add(new NpgsqlParameter($"@unit_cost{i}", order.UnitCost.Value));
            parameters.Add(new NpgsqlParameter($"@total_revenue{i}", order.TotalRevenue.Value));
            parameters.Add(new NpgsqlParameter($"@total_cost{i}", order.TotalCost.Value));
            parameters.Add(new NpgsqlParameter($"@total_profit{i}", order.TotalProfit.Value));
        }

        sql.Append(
            " ON CONFLICT (order_id) DO UPDATE SET " +
            "uuid = EXCLUDED.uuid, " +
            "region = EXCLUDED.region, " +
            "country = EXCLUDED.country, " +
            "item_type = EXCLUDED.item_type, " +
            "sales_channel = EXCLUDED.sales_channel, " +
            "order_priority = EXCLUDED.order_priority, " +
            "order_date = EXCLUDED.order_date, " +
            "ship_date = EXCLUDED.ship_date, " +
            "units_sold = EXCLUDED.units_sold, " +
            "unit_price = EXCLUDED.unit_price, " +
            "unit_cost = EXCLUDED.unit_cost, " +
            "total_revenue = EXCLUDED.total_revenue, " +
            "total_cost = EXCLUDED.total_cost, " +
            "total_profit = EXCLUDED.total_profit;");

        return (sql.ToString(), parameters);
    }
}