using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConveyorBelt.Tooling.Events;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ConveyorBelt.Tooling.Querying
{
    public class TableStorageShardKeyQuery : IShardKeyQuery
    {
        public Task<IEnumerable<DynamicTableEntity>> QueryAsync(ShardKeyArrived shardKeyArrived)
        {
            var account = CloudStorageAccount.Parse(shardKeyArrived.Source.ConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference(shardKeyArrived.Source.DynamicProperties["TableName"].ToString());
            var shardKeyEnd = long.Parse(shardKeyArrived.ShardKey) + 10000000;
            var Filter = new TableQuery().Where(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", "ge", new DateTime(long.Parse(shardKeyArrived.ShardKey)).ToString("yyyyMMdd-hhmms")),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PartitionKey", "le", new DateTime(shardKeyEnd).ToString("yyyyMMdd-hhmms"))));
            return Task.FromResult(table.ExecuteQuery(new TableQuery().Where(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", "ge", new DateTime(long.Parse(shardKeyArrived.ShardKey)).ToString("yyyyMMdd-hhmms")), 
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PartitionKey", "le", new DateTime(shardKeyEnd).ToString("yyyyMMdd-hhmms"))))));
        }
    }
}
