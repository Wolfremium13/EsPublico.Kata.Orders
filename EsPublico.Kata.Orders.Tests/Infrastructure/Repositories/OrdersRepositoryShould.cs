using EsPublico.Kata.Orders.Infrastructure.Config;
using EsPublico.Kata.Orders.Infrastructure.Databases;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using EsPublico.Kata.Orders.Tests.Domain.Builders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace EsPublico.Kata.Orders.Tests.Infrastructure.Repositories;

public class OrdersRepositoryShould : IDisposable
{
    private readonly PostgresAdapter _adapter;
    private readonly PostgresOrdersRepository _repository;

    public OrdersRepositoryShould()
    {
        var databaseSettings = new DatabaseSettings
        {
            ConnectionString = "Host=127.0.0.1:5432;Username=espublico;Password=espublico;Database=espublico"
        };
        _adapter = new PostgresAdapter(databaseSettings);
        var logger = Substitute.For<ILogger<PostgresOrdersRepository>>();
        _repository = new PostgresOrdersRepository(_adapter, logger);
    }

    public async void Dispose()
    {
        await _adapter.Execute(async cmd =>
        {
            cmd.CommandText = "DELETE FROM orders WHERE order_id = 1";
            await cmd.ExecuteNonQueryAsync();
        });
    }

    // [Fact]
    [Fact(Skip = "Integration test: Needs to launch docker compose postgres")]
    public async Task Save()
    {
        var aOrder = new OrderBuilder().WithId(1).Build();
        var orders = new OrdersBuilder().WithOrder(aOrder).Build();

        var result = await _repository.Save(orders.Value);

        result.Match(
            _ => { },
            error => throw new Exception($"Error saving orders: {error.Message}")
        );
    }

    // [Fact]
    [Fact(Skip = "Integration test: Needs to launch docker compose postgres")]
    public async Task NotAllowDuplicatedOrderId()
    {
        var aOrder = new OrderBuilder().WithId(1).Build();
        var duplicatedOrder = new OrderBuilder().WithId(1).Build();
        var orders = new OrdersBuilder()
            .WithOrder(aOrder)
            .WithOrder(duplicatedOrder)
            .Build();

        var result = await _repository.Save(orders.Value);

        result.Match(
            _ => { },
            error => error.Message.Should().Contain("ON CONFLICT DO UPDATE")
        );
    }
}