namespace EsPublico.Kata.Orders.Domain;

public class Orders
{
    private Orders(List<OrderItems.Order> givenOrders, NextOrdersLink givenNextOrdersLink)
    {
        OrdersList = givenOrders;
        NextOrdersLink = givenNextOrdersLink;
    }

    public List<OrderItems.Order> OrdersList { get; }
    public NextOrdersLink NextOrdersLink { get; }
}