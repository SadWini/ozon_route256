using Domain.Dao;

public interface IGoodsRepository
{
    Guid Add(GoodItemDao goodItem);

    IReadOnlyList<GoodItemDao> Find(Filter filter);

    GoodItemDao Find(Guid id);

    GoodItemDao UpdatePrice(Guid id, decimal newPrice);
}
