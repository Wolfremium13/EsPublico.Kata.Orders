namespace EsPublico.Kata.Orders.Domain;

public record Orders(List<OrderItems.Order> Value);

public record OrdersWithNextPage(List<OrderItems.Order> Value, NextOrdersLink NextOrdersLink) : Orders(Value);