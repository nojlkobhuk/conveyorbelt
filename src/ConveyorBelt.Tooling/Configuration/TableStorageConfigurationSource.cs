﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeHive.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ConveyorBelt.Tooling.Configuration
{
    public class TableStorageConfigurationSource : ISourceConfiguration
    {
        private CloudTable _table;
        private object _lock = new object();

        public TableStorageConfigurationSource(IConfigurationValueProvider configurationValueProvider)
        {
            MakeSureTableIsThere(configurationValueProvider);
        }

        public IEnumerable<DiagnosticsSource> GetSources()
        {
            return _table.ExecuteQuery(new TableQuery<DynamicTableEntity>()).Select(x => new DiagnosticsSource(x));
        }

        public void UpdateSource(DiagnosticsSource source)
        {
            _table.Execute(TableOperation.InsertOrReplace(source.ToEntity()));
        }

        private void MakeSureTableIsThere(IConfigurationValueProvider configurationValueProvider)
        {
            if (_table == null)
            {
                lock (_lock)
                {
                    if (_table == null)
                    {
                        var account = CloudStorageAccount.Parse(configurationValueProvider.GetValue(ConfigurationKeys.StorageConnectionString));
                        var client = account.CreateCloudTableClient();
                        _table = client.GetTableReference(configurationValueProvider.GetValue(ConfigurationKeys.TableName));
                        _table.CreateIfNotExists();
                    }
                }
            }        
        }
    }
}
