using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using KafkaHomework.OrderEventConsumer.Infrastructure;
using Microsoft.Extensions.Logging;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;
using OrderEvent = KafkaHomework.OrderEventConsumer.Domain.Order.OrderEvent;

namespace KafkaHomework.OrderEventConsumer.Presentation;

public class ItemHandler : IHandler<Ignore, string>
{
    private readonly IItemStatsRepository _itemStatsRepository;
    private readonly ILogger<ItemHandler> _logger;

    public ItemHandler(IItemStatsRepository itemStatsRepository, ILogger<ItemHandler> logger)
    {
        _itemStatsRepository = itemStatsRepository;
        _logger = logger;
    }

    public async Task Handle(IReadOnlyCollection<ConsumeResult<Ignore, string>> messages, CancellationToken token)
    {
        foreach (var message in messages)
        {
            if (token.IsCancellationRequested)
            {
                _logger.LogInformation("Cancellation requested, stopping message processing.");
                break;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };
                var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message.Message.Value, options);

                if (orderEvent == null)
                {
                    _logger.LogWarning("Received null OrderEvent for message offset {Offset}", message.Offset);
                    continue;
                }

                foreach (var position in orderEvent.Positions)
                {
                    switch (orderEvent.Status)
                    {
                        case Status.Created:
                            await _itemStatsRepository.ReserveItemAsync(position.ItemId.Value, position.Quantity, DateTime.UtcNow);
                            break;
                        case Status.Delivered:
                            await _itemStatsRepository.SellItemAsync(position.ItemId.Value, position.Quantity, DateTime.UtcNow);
                            break;
                        case Status.Canceled:
                            await _itemStatsRepository.CancelItemAsync(position.ItemId.Value, position.Quantity, DateTime.UtcNow);
                            break;
                        default:
                            _logger.LogWarning("Unknown order status: {Status} for message offset {Offset}", orderEvent.Status, message.Offset);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message offset {Offset}", message.Offset);
            }
        }
    }
}
