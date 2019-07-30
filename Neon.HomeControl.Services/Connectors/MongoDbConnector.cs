using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Neon.HomeControl.Api.Core.Attributes.Database;
using Neon.HomeControl.Api.Core.Interfaces.Database;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;

namespace Neon.HomeControl.Services.Connectors
{
	[NoSqlConnector("mongodb")]
	public class MongoDbConnector : INoSqlConnector
	{

		private readonly ILogger _logger;
		private MongoClient _mongoClient;
		private IMongoDatabase _mongoDatabase;

		public MongoDbConnector(ILogger<MongoDbConnector> logger)
		{
			_logger = logger;
		}
		public Task<bool> Init(string connectionString)
		{

			var mongoUrl = new MongoUrl(connectionString);

			_logger.LogInformation($"Connecting to MongoDB [database {mongoUrl.DatabaseName}]");
			_mongoClient = new MongoClient(connectionString);
			_mongoDatabase = _mongoClient.GetDatabase(mongoUrl.DatabaseName);

			return Task.FromResult(true);
		}

		public T Insert<T>(T value, string collectionName) where T : IIotEntity
		{
			value.Id = Guid.NewGuid();
			_mongoDatabase.GetCollection<T>(collectionName).InsertOne(value);

			return value;
		}

		public T Update<T>(T value, string collectionName) where T : IIotEntity
		{
			_mongoDatabase.GetCollection<T>(collectionName).ReplaceOne(entity => entity.Id == value.Id, value);

			return value;
		}

		public List<T> List<T>(string collectionName) where T : IIotEntity
		{
			return _mongoDatabase.GetCollection<T>(collectionName).FindSync(entity => true).ToList();
		}

		public bool CollectionExists(string name)
		{
			try
			{
				_mongoDatabase.CreateCollection(name);

				return false;
			}
			catch
			{
				return true;
			}
		}

		public bool AddIndex(string collectionName, string field, bool unique)
		{
			var keys = Builders<BsonDocument>.IndexKeys.Ascending(field);
			_mongoDatabase.GetCollection<BsonDocument>(collectionName).Indexes.CreateOne(new CreateIndexModel<BsonDocument>(keys));

			return false;
		}

		public IQueryable<T> Query<T>(string collectionName) where T : IIotEntity
		{
			return _mongoDatabase.GetCollection<T>(collectionName).AsQueryable();
		}

		public void Dispose()
		{
			_mongoDatabase = null;
			_mongoClient = null;
		}

	}
}
