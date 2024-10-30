using System;
using FluentMigrator;

using KafkaHomework.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(1, "Initial migration")]
public sealed class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => 
        @"create table if not exists items_stats (
                 item_id bigint primary key
                , reserved_count int not null default 0
                , sold_count int not null default 0
                , canceled_count int not null default 0
                , last_updated timestamptz not null
        );
";
}
