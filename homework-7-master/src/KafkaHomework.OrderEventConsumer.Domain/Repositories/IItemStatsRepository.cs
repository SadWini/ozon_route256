using System;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure;

public interface IItemStatsRepository
{
    Task ReserveItemAsync(long itemId, int quantity, DateTimeOffset lastUpdated);
    Task SellItemAsync(long itemId, int quantity, DateTimeOffset lastUpdated);
    Task CancelItemAsync(long itemId, int quantity, DateTimeOffset lastUpdated);
}