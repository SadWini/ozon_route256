using AutoFixture;
using Domain.Dao;
using Xunit;
using Infrastructure;

namespace UnitTests;

public class GoodsRepositoryTests
{
    private readonly IGoodsRepository _repository = new GoodsRepository();
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void Test_AddItem()
    {
        var goodItem = new GoodItemDao {
            Price = 10.0m,
            Type = GoodItemType.Common,
            CreatedAt = DateTime.UtcNow,
            StorageNumber = 1 };
        var id = _repository.Add(goodItem);

        Assert.Equal(0, id);
        var retrievedItem = _repository.Find(id);
        Assert.Equal(goodItem.Price, retrievedItem.Price);
    }

    [Fact]
    public void Test_FindWithFilter()
    {
        var goodItem1 = _fixture.Build<GoodItemDao>()
            .With(x => x.Type, GoodItemType.Common)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.StorageNumber, 1)
            .Create();
        var goodItem2 = _fixture.Build<GoodItemDao>()
            .With(x => x.Type, GoodItemType.Food)
            .Create();
        var goodItem3 = _fixture.Build<GoodItemDao>()
            .With(x => x.Type, GoodItemType.Common)
            .With(x => x.CreatedAt, DateTime.UtcNow)
            .With(x => x.StorageNumber, 1)
            .Create();

        _repository.Add(goodItem1);
        _repository.Add(goodItem2);
        _repository.Add(goodItem3);

        var filter = new Filter {
            ItemType = GoodItemType.Common,
            CreatedAt = DateTime.UtcNow.AddMinutes(-20),
            StorageNumber = 1 };

        var filteredItems = _repository.Find(filter);

        Assert.Equal(2, filteredItems.Count());
        Assert.Contains(filteredItems, item => item.Price == goodItem1.Price);
        Assert.Contains(filteredItems, item => item.Price == goodItem3.Price);
    }

    [Fact]
    public void Test_FindItem_WhenExists()
    {
        var goodItem = _fixture.Create<GoodItemDao>();
        var id = _repository.Add(goodItem);
        var foundItem = _repository.Find(id);

        Assert.NotNull(foundItem);
        Assert.Equal(goodItem.Price, foundItem.Price);
    }

    [Fact]
    public void Test_FindItem_WhenItemDoesNotExist()
    {
        Assert.Throws<NotFoundException>(() => _repository.Find(999));
    }

    [Fact]
    public void Test_UpdatePrice_WhenItemExists()
    {
        var goodItem = _fixture.Create<GoodItemDao>();
        var id = _repository.Add(goodItem);
        var newPrice = goodItem.Price * 2;
        var updatedItem = _repository.UpdatePrice(id, newPrice);

        Assert.Equal(newPrice, updatedItem.Price);
    }

    [Fact]
    public void Test_UpdatePrice_WhenItemDoesNotExist()
    {
        Assert.Throws<NotFoundException>(() => _repository.UpdatePrice(-1, 20.0m));
    }
}
