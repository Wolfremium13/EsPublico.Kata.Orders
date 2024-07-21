using EsPublico.Kata.Orders.Infrastructure.Config;
using EsPublico.Kata.Orders.Infrastructure.Databases;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Infrastructure.Repositories;

public class PostgresSummaryRepositoryShould
{
    private readonly PostgresSummaryRepository _repository;

    public PostgresSummaryRepositoryShould()
    {
        var databaseSettings = new DatabaseSettings
        {
            ConnectionString = "Host=127.0.0.1:5432;Username=espublico;Password=espublico;Database=espublico"
        };
        var adapter = new PostgresAdapter(databaseSettings);
        _repository = new PostgresSummaryRepository(adapter);
    }

    [Fact(Skip = "Integration test: Needs data and to launch docker compose postgres")]
    // [Fact]
    public async Task GetRecords()
    {
        var records = await _repository.GetSummary();

        records.Match(
            summaryRecords => summaryRecords.Should().NotBeEmpty(),
            error => error.Should().BeNull()
        );
    }
}