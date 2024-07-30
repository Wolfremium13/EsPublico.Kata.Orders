using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class OrderShould
{
    private const long ValidId = 443368995;
    private const string? ValidRegion = "Sub-Saharan Africa";
    private const string? ValidCountry = "South Africa";
    private const string? ValidItemType = "Fruits";
    private const string? ValidSalesChannel = "Offline";
    private const string? ValidPriority = "M";
    private const string? ValidDate = "7/27/2012";
    private const string? ValidShipDate = "7/28/2012";
    private const long ValidUnitsSold = 1593;
    private const decimal ValidUnitPrice = 9.33m;
    private const decimal ValidUnitCost = 6.92m;
    private const decimal ValidTotalRevenue = 14862.69m;
    private const decimal ValidTotalCost = 11023.56m;
    private const decimal ValidTotalProfit = 3839.13m;

    [Fact]
    public void BeCreated()
    {
        const string? invalidUuid = "invalid-uuid";

        var invalidOrder = Order.Create(
            invalidUuid,
            ValidId,
            ValidRegion,
            ValidCountry,
            ValidItemType,
            ValidSalesChannel,
            ValidPriority,
            ValidDate,
            ValidShipDate,
            ValidUnitsSold,
            ValidUnitPrice,
            ValidUnitCost,
            ValidTotalRevenue,
            ValidTotalCost,
            ValidTotalProfit
        );

        invalidOrder.Match(
            order => order.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid UUID")
        );
    }

    [Fact]
    public void NotAllowWrongAnyWrongParameter()
    {
        const string? validUuid = "1858f59d-8884-41d7-b4fc-88cfbbf00c53";

        var validOrder = Order.Create(
            validUuid,
            ValidId,
            ValidRegion,
            ValidCountry,
            ValidItemType,
            ValidSalesChannel,
            ValidPriority,
            ValidDate,
            ValidShipDate,
            ValidUnitsSold,
            ValidUnitPrice,
            ValidUnitCost,
            ValidTotalRevenue,
            ValidTotalCost,
            ValidTotalProfit
        );

        validOrder.Match(
            order =>
            {
                order.Uuid.Value.Should().Be(validUuid);
                order.Id.Value.Should().Be(ValidId);
                order.Region.Value.Should().Be(ValidRegion);
                order.Country.Value.Should().Be(ValidCountry);
                order.ItemType.ToString().Should().Be(ValidItemType);
                order.SalesChannel.ToString().Should().Be(ValidSalesChannel);
                order.Priority.Value.Should().Be(ValidPriority);
                order.Date.ToString().Should().Be("27-07-2012");
                order.ShipDate.ToString().Should().Be("28-07-2012");
                order.UnitsSold.Value.Should().Be(ValidUnitsSold);
                order.UnitPrice.Value.Should().Be(ValidUnitPrice);
                order.UnitCost.Value.Should().Be(ValidUnitCost);
                order.TotalRevenue.Value.Should().Be(ValidTotalRevenue);
                order.TotalCost.Value.Should().Be(ValidTotalCost);
                order.TotalProfit.Value.Should().Be(ValidTotalProfit);
            },
            error => error.Should().BeNull()
        );
    }
}