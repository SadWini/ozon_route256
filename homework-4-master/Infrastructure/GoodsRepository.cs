using Domain.Dao;

namespace Infrastructure;

public class GoodsRepository : IGoodsRepository
{
    private readonly Dictionary<int,GoodItemDao>  _goodItems = new();
    private readonly ReaderWriterLock _lock = new();
    private readonly int _timeout = 70;
    private int _id = 0;
    public int Add(GoodItemDao goodItem)
    {
        try
        {
            _lock.AcquireWriterLock(_timeout);
            var index = _id;
            Interlocked.Increment(ref _id);
            goodItem.Id = index;
            if (_goodItems.TryAdd(index, goodItem))
                return index;
            throw new InvalidOperationException("Item already exists.");
        }
        finally
        {
            _lock.ReleaseWriterLock();
        }
    }

    public GoodItemDao Find(int id)
    {
        try
        {
            _lock.AcquireReaderLock(_timeout);
            if (!_goodItems.TryGetValue(id, out GoodItemDao? res))
            {
                throw new NotFoundException();
            }
            return res;
        }
        finally
        {
            _lock.ReleaseReaderLock();
        }
    }

    public IReadOnlyList<GoodItemDao> Find(Filter filter)
    {
        try
        {
            _lock.AcquireReaderLock(_timeout);
            return _goodItems.Values.Where(x => x.Type == filter.ItemType)
                .Where(x => x.CreatedAt > filter.CreatedAt)
                .Where(x => x.StorageNumber == filter.StorageNumber)
                .ToList();
        }
        finally
        {
            _lock.ReleaseReaderLock();
        }
    }

    public GoodItemDao UpdatePrice(int id, decimal newPrice)
    {
        try
        {
            _lock.AcquireWriterLock(_timeout);
            var item = Find(id);
            _goodItems[id].Price = newPrice;
            return item;
        }
        finally
        {
            _lock.ReleaseWriterLock();
        }
    }
}
