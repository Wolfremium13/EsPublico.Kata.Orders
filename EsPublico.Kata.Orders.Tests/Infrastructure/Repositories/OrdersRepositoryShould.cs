using EsPublico.Kata.Orders.Infrastructure.Config;
using EsPublico.Kata.Orders.Infrastructure.Databases;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using EsPublico.Kata.Orders.Tests.Domain.Builders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace EsPublico.Kata.Orders.Tests.Infrastructure.Repositories;

public class OrdersRepositoryShould
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

    // [Fact]
    [Fact(Skip = "Integration test")]
    public async Task Save()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var orders = new OrdersBuilder().WithOrder(aOrder).Build();

        var result = await _repository.Save(orders.Value);

        result.Match(
            _ => { },
            error => throw new Exception($"Error saving orders: {error.Message}")
        );
    }

    // [Fact]
    [Fact(Skip = "Integration test")]
    public async Task UpdatesIfExists()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var orders = new OrdersBuilder().WithOrder(aOrder).Build();

        await _repository.Save(orders.Value);
        var result = await _repository.Save(orders.Value);

        await result.Match(async _ =>
            {
                await _adapter.Execute(async cmd =>
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM orders";
                    var count = (long)(await cmd.ExecuteScalarAsync() ?? throw new InvalidOperationException());
                    count.Should().Be(1);
                });
            },
            error => throw new Exception($"Error saving orders: {error.Message}")
        );
    }
}