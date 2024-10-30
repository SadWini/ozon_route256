using Domain.Dao;

public interface IGoodsRepository
{
    int Add(GoodItemDao goodItem);

    IReadOnlyList<GoodItemDao> Find(Filter filter);

    GoodItemDao Find(int id);

    GoodItemDao UpdatePrice(int id, decimal newPrice);
}
