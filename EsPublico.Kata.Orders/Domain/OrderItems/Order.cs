using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class Order
{
    private Order(
        Uuid uuid,
        Id id,
        Region region,
        Country country,
        ItemType itemType,
        SalesChannel salesChannel,
        Priority priority,
        Date date,
        ShipDate shipDate,
        UnitsSold unitsSold,
        UnitPrice unitPrice,
        UnitCost unitCost,
        TotalRevenue totalRevenue,
        TotalCost totalCost,
        TotalProfit totalProfit
    )
    {
        Uuid = uuid;
        Id = id;
        Region = region;
        Country = country;
        ItemType = itemType;
        SalesChannel = salesChannel;
        Priority = priority;
        Date = date;
        ShipDate = shipDate;
        UnitsSold = unitsSold;
        UnitPrice = unitPrice;
        UnitCost = unitCost;
        TotalRevenue = totalRevenue;
        TotalCost = totalCost;
        TotalProfit = totalProfit;
    }

    public Uuid Uuid { get; }
    public Id Id { get; }
    public Region Region { get; }
    public Country Country { get; }
    public ItemType ItemType { get; }
    public SalesChannel SalesChannel { get; }
    public Priority Priority { get; }
    public Date Date { get; }
    public ShipDate ShipDate { get; }
    public UnitsSold UnitsSold { get; }
    public UnitPrice UnitPrice { get; }
    public UnitCost UnitCost { get; }
    public TotalRevenue TotalRevenue { get; }
    public TotalCost TotalCost { get; }
    public TotalProfit TotalProfit { get; }


    public static Either<Error, Order> Create(
        string? uuid,
        long id,
        string? region,
        string? country,
        string? itemType,
        string? salesChannel,
        string? priority,
        string? date,
        string? shipDate,
        long unitsSold,
        decimal unitPrice,
        decimal unitCost,
        decimal totalRevenue,
        decimal totalCost,
        decimal totalProfit
    )
    {
        return
            from uuidValue in Uuid.Create(uuid)
            from idValue in Id.Create(id)
            from regionValue in Region.Create(region)
            from countryValue in Country.Create(country)
            from itemTypeValue in ItemType.Create(itemType)
            from salesChannelValue in SalesChannel.Create(salesChannel)
            from priorityValue in Priority.Create(priority)
            from dateValue in Date.Create(date)
            from shipDateValue in ShipDate.Create(shipDate)
            from unitsSoldValue in UnitsSold.Create(unitsSold)
            from unitPriceValue in UnitPrice.Create(unitPrice)
            from unitCostValue in UnitCost.Create(unitCost)
            from totalRevenueValue in TotalRevenue.Create(totalRevenue)
            from totalCostValue in TotalCost.Create(totalCost)
            from totalProfitValue in TotalProfit.Create(totalProfit)
            select new Order(
                uuidValue,
                idValue,
                regionValue,
                countryValue,
                itemTypeValue,
                salesChannelValue,
                priorityValue,
                dateValue,
                shipDateValue,
                unitsSoldValue,
                unitPriceValue,
                unitCostValue,
                totalRevenueValue,
                totalCostValue,
                totalProfitValue
            );
    }
}