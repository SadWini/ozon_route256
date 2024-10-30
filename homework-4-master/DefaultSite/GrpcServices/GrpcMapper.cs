using DefaultSite.GrpcService;
using Domain.Dao;

namespace DefaultSite.GrpcServices;

public class GrpcMapper
{
    public static GoodItemDao Map(AddGoodRequest request)
    {
        return new GoodItemDao
        {
            Price = (decimal) request.GoodItem.Price,
            Name = request.GoodItem.Name,
            Weight = (decimal) request.GoodItem.Weight,
            CreatedAt = DateTime.Parse(request.GoodItem.CreatedAt),
            Type = MapToGoodItemType(request.GoodItem.Type),
            StorageNumber = request.GoodItem.StorageNumber
        };
    }

    public static DefaultSite.GrpcService.GoodItemDto Map(GoodItemDao item)
    {
        return new DefaultSite.GrpcService.GoodItemDto
        {
            Price = (float)item.Price,
            Name = item.Name,
            Weight = (float)item.Weight,
            CreatedAt = item.CreatedAt.ToString(),
            Type = MapToGrpcItemType(item.Type),
            StorageNumber = item.StorageNumber
        };
    }

    public static FilterItemDto Map(Filter filter)
    {
        return new FilterItemDto
        {
            CreatedAt = filter.CreatedAt.ToString(),
            Type = MapToGrpcItemType(filter.ItemType),
            StorageNumber = filter.StorageNumber
        };
    }

    public static UpdateGoodItemPrice Map(UpdateItemPriceRequest request)
    {
        return new UpdateGoodItemPrice
        {
            Id = request.Id,
            Price = (decimal)request.Price
        };
    }

    public static Filter Map(FilterRequest request)
    {
        return new Filter
        {
            CreatedAt = DateTime.Parse(request.Item.CreatedAt),
            StorageNumber = request.Item.StorageNumber,
            ItemType = MapToGoodItemType(request.Item.Type)
        };
    }

    public static GoodItemType MapToGoodItemType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Common:
                return GoodItemType.Common;
            case ItemType.Food:
                return GoodItemType.Food;
            case ItemType.HouseholdChemicals:
                return GoodItemType.HouseholdChemicals;
            case ItemType.Technics:
                return GoodItemType.Technics;
            default:
                throw new ArgumentException($"Can't parse item type from {type}");
        }
    }

    public static ItemType MapToGrpcItemType(GoodItemType type)
    {
        switch (type)
        {
            case GoodItemType.Common:
                return ItemType.Common;
            case GoodItemType.Food:
                return ItemType.Food;
            case GoodItemType.HouseholdChemicals:
                return ItemType.HouseholdChemicals;
            case GoodItemType.Technics:
                return ItemType.Technics;
            default:
                throw new ArgumentException($"Can't parse item type from {type}");
        }
    }
}
