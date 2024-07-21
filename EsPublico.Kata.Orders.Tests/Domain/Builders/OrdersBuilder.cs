using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;

namespace EsPublico.Kata.Orders.Tests.Domain.Builders;

public class OrdersBuilder
{
    private readonly List<Order> _orders = [];
    private NextOrdersLink? _nextOrdersLink;

    public OrdersBuilder WithOrder(Order order)
    {
        _orders.Add(order);
        return this;
    }

    public OrdersBuilder WithNextOrdersLink(NextOrdersLink nextOrdersLink)
    {
        _nextOrdersLink = nextOrdersLink;
        return this;
    }

    public Orders.Domain.Orders Build()
    {
        return _nextOrdersLink is null
            ? new Orders.Domain.Orders(_orders)
            : new OrdersWithNextPage(_orders, _nextOrdersLink);
    }
}