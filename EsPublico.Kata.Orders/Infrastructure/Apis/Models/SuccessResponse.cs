namespace EsPublico.Kata.Orders.Infrastructure.Apis.Models;

public class SuccessResponse
{
    public int Page { get; set; }
    public List<OrderResponse> Content { get; set; }
    public LinksResponse LinksResponse { get; set; }
}

public class OrderResponse
{
    public string Uuid { get; set; }
    public long Id { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string ItemType { get; set; }
    public string SalesChannel { get; set; }
    public string Priority { get; set; }
    public string Date { get; set; }
    public string ShipDate { get; set; }
    public long UnitsSold { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalProfit { get; set; }
    public LinksResponse LinksResponse { get; set; }
}

public class LinksResponse
{
    public string Self { get; set; }
    public string Next { get; set; }
}