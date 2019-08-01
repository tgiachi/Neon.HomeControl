using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Database;
using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Interfaces.Database;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Services.Connectors
{
	[NoSqlConnector("litedb")]
	public class LiteDbConnector : INoSqlConnector
	{
		private readonly ILogger _logger;
		private LiteDatabase _liteDatabase;
		private IFileSystemManager _fileSystemManager;
		public LiteDbConnector(ILogger<LiteDbConnector> logger, IFileSystemManager fileSystemManager)
		{
			_logger = logger;
			_fileSystemManager = fileSystemManager;
		}

		/// <summary>
		/// Initialize NoSQL db
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public Task<bool> Init(string connectionString)
		{
			_logger.LogInformation($"Loading {connectionString}");
			_fileSystemManager.CreateDirectory(Path.GetDirectoryName(connectionString));

			_liteDatabase = new LiteDatabase(_fileSystemManager.BuildFilePath(connectionString));

			_liteDatabase.Shrink();
			_logger.LogInformation("Database loaded");

			return Task.FromResult(true);
		}

		public T Insert<T>(T value, string collectionName) where T : IIotEntity
		{

			lock (_liteDatabase)
			{
				value.Id = Guid.NewGuid();

				var collection = _liteDatabase.GetCollection<T>(collectionName);
				collection.Insert(value);

				return value;
			}
		}

		public T Update<T>(T value, string collectionName) where T : IIotEntity
		{
			lock (_liteDatabase)
			{
				var collection = _liteDatabase.GetCollection<T>(collectionName);
				collection.Update(value);

				return value;
			}
		}

		public List<T> List<T>(string collectionName) where T : IIotEntity
		{
			lock (_liteDatabase)
			{
				return _liteDatabase.GetCollection<T>(collectionName).FindAll().ToList();

			}
		}



		public bool CollectionExists(string name)
		{
			return _liteDatabase.CollectionExists(name);
		}

		public bool AddIndex(string collectionName, string field, bool unique)
		{
			return _liteDatabase.GetCollection(collectionName).EnsureIndex(field, unique);
		}

		public IQueryable<T> Query<T>(string collectionName) where T : IIotEntity
        {
            return _liteDatabase.GetCollection<T>(collectionName).FindAll().AsQueryable();

        }

		public void Dispose()
		{
			_liteDatabase?.Dispose();
		}
	}
}
