using EsPublico.Kata.Orders.Domain.OrderItems;

namespace EsPublico.Kata.Orders.Tests.Domain.Builders;

public class OrderBuilder
{
    private long _id = 1;
    private string? _uuid = "1858f59d-8884-41d7-b4fc-88cfbbf00c53";

    public OrderBuilder WithUuid(string? givenUuid)
    {
        _uuid = givenUuid;
        return this;
    }

    public OrderBuilder WithId(long id)
    {
        _id = id;
        return this;
    }

    public Order Build()
    {
        return Order.Create(
            uuid: _uuid,
            id: _id,
            region: "Europe",
            country: "Portugal",
            itemType: "Technology",
            salesChannel: "Online",
            priority: "H",
            date: "01/06/2021",
            shipDate: "01/06/2021",
            unitsSold: 1,
            unitPrice: 1,
            unitCost: 1,
            totalRevenue: 1,
            totalCost: 1,
            totalProfit: 1
        ).Match(
            Right: order => order,
            Left: error => throw new Exception(error.Message)
        );
    }
}