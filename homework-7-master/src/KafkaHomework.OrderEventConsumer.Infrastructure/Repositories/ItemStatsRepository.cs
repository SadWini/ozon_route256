using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using KafkaHomework.OrderEventConsumer.Infrastructure;

namespace KafkaHomework.OrderEventConsumer.Presentation.Common;

public class ItemStatsRepository : IItemStatsRepository
{
    private readonly IDbConnection _dbConnection;

    public ItemStatsRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task ReserveItemAsync(long itemId, int quantity, DateTimeOffset lastUpdated)
    {
        var sql = @"
            insert into items_stats (item_id, reserved_count, last_updated)
            values (@ItemId, @Quantity, @LastUpdated)
                on CONFLICT(item_id) do update
               set reserved_count = items_stats.reserved_count + @Quantity
                   , last_updated = @LastUpdated;";

        await _dbConnection.ExecuteAsync(sql, new
        {
            ItemId = itemId,
            Quantity = quantity,
            LastUpdated = lastUpdated
        });
    }

    public async Task SellItemAsync(long itemId, int quantity, DateTimeOffset lastUpdated)
    {
        var sql = @"
            update items_stats
               set reserved_count = reserved_count - @Quantity
                   , old_count = sold_count + @Quantity
                   , last_updated = @LastUpdated
             where item_id = @ItemId;";

        await _dbConnection.ExecuteAsync(sql, new
        {
            ItemId = itemId,
            Quantity = quantity,
            LastUpdated = lastUpdated
        });
    }

    public async Task CancelItemAsync(long itemId, int quantity, DateTimeOffset lastUpdated)
    {
        var sql = @"
         update items_stats
            set reserved_count = reserved_count - @Quantity
                , canceled_count = canceled_count + @Quantity
                , last_updated = @LastUpdated
          where item_id = @ItemId;";

        await _dbConnection.ExecuteAsync(sql, new
        {
            ItemId = itemId,
            Quantity = quantity,
            LastUpdated = lastUpdated
        });
    }
}