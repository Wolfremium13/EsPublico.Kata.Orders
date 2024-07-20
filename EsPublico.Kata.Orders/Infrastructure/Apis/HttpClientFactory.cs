namespace EsPublico.Kata.Orders.Infrastructure.Apis;

public interface IHttpClientFactory
{
    HttpClient CreateClient();
}