namespace EsPublico.Kata.Orders.Infrastructure.Apis;

public interface IHttpClientFactory
{
    HttpClient CreateClient();
}

public class HttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient() => new HttpClient();
}