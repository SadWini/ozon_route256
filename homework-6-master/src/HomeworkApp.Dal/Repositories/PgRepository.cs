using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Npgsql;

namespace HomeworkApp.Dal.Repositories;

public abstract class PgRepository : IPgRepository
{
    private readonly DalOptions _dalSettings;

    protected const int DefaultTimeoutInSeconds = 5;

    protected PgRepository(DalOptions dalSettings)
    {
        _dalSettings = dalSettings;
    }

    protected async Task<NpgsqlConnection> GetConnection()
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException("Transaction was aborted (probably by user cancellation request)");
        }

        var connection = new NpgsqlConnection(_dalSettings.PostgresConnectionString);
        await connection.OpenAsync();

        // Due to in-process migrations
        connection.ReloadTypes();

        return connection;
    }
    
    public async Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token)
    {
        const string sqlQuery = @"
    select id
         , task_id
         , author_user_id
         , message
         , at
         , modified_at
         , deleted_at
      from task_comments
     where task_id = @TaskId
       and (@IncludeDeleted = true 
        or deleted_at is null)
     order by at desc;
    ";

        await using var connection = await GetConnection();
        var results = await connection.QueryAsync<TaskCommentEntityV1>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    model.TaskId, 
                    model.IncludeDeleted
                },
                cancellationToken: token));

        return results.ToArray();
    }
}
