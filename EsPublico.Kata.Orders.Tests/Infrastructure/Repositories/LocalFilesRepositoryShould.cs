using EsPublico.Kata.Orders.Domain.OrderItems;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using EsPublico.Kata.Orders.Tests.Domain.Builders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace EsPublico.Kata.Orders.Tests.Infrastructure.Repositories;

public class LocalFilesRepositoryShould : IDisposable
{
    private readonly DateTime _executionDate;
    private readonly string _folderPath;
    private readonly LocalFilesRepository _repository;

    public LocalFilesRepositoryShould()
    {
        var logger = Substitute.For<ILogger<LocalFilesRepository>>();
        _repository = new LocalFilesRepository(logger);
        _executionDate = DateTime.Now;
        _folderPath = Path.Combine(Path.GetTempPath(), $"kata-orders-{_executionDate:yyyy-MM-ddTHH-mm-ss}");
    }

    public void Dispose()
    {
        if (Directory.Exists(_folderPath))
        {
            Directory.Delete(_folderPath, true);
        }
    }

    [Fact]
    public async Task Save()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var orders = new List<Order> { aOrder };

        var saveResult = await _repository.Save(orders, _executionDate);

        saveResult.Match(
            _ =>
            {
                var files = Directory.GetFiles(_folderPath, "orders_*.csv");
                files.Should().HaveCount(1);
            },
            error => throw new Exception($"Error saving orders: {error.Message}")
        );
    }

    [Fact]
    public async Task SaveMultipleOrders()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var anotherOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c54").Build();
        var orders = new List<Order> { aOrder, anotherOrder };

        var saveResult = await _repository.Save(orders, _executionDate);

        saveResult.Match(
            _ =>
            {
                var files = Directory.GetFiles(_folderPath, "orders_*.csv");
                files.Should().HaveCount(1);
                var fileContent = File.ReadAllText(files.First());
                fileContent.Should().Contain(aOrder.Uuid.ToString());
                fileContent.Should().Contain(anotherOrder.Uuid.ToString());
            },
            error => throw new Exception($"Error saving orders: {error.Message}")
        );
    }

    [Fact]
    public async Task HandleErrors()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var orders = new List<Order> { aOrder };
        var exceptionMessage = "Some error during saving";

        var result = await _repository.Save(orders, _executionDate);

        result.Match(
            _ => throw new Exception("Should have failed"),
            error => error.Message.Should().Be(exceptionMessage)
        );
    }
}