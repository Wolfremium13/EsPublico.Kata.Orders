using EsPublico.Kata.Orders.Infrastructure.Config;
using Npgsql;

namespace EsPublico.Kata.Orders.Infrastructure.Databases;

public class PostgresAdapter(DatabaseSettings settings)
{
    private readonly string? _connectionString = settings.ConnectionString;

    public async Task<T> Get<T>(Func<NpgsqlCommand, Task<T>> func)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var command = conn.CreateCommand();
        return await func(command);
    }

    public async Task Execute(Func<NpgsqlCommand, Task> func)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var command = conn.CreateCommand();
        await func(command);
    }
}