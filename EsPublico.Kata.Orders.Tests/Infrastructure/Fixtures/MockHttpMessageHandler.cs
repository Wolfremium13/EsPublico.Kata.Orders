namespace EsPublico.Kata.Orders.Tests.Infrastructure.Fixtures;

public class MockHttpMessageHandler(HttpResponseMessage httpResponseMessage) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(httpResponseMessage);
    }
}