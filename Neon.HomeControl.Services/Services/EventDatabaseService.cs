using LiteDB;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Database;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;
using Neon.HomeControl.Api.Core.Interfaces.Database;
using Neon.HomeControl.Api.Core.Interfaces.Managers;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IEventDatabaseService), LoadAtStartup = true, Order = 3)]
	public class EventDatabaseService : IEventDatabaseService
	{
		private readonly NeonConfig _config;
		private readonly string _dbFilename = "Neon.HomeControl.Events.db";

		private readonly Dictionary<Type, string> _entities = new Dictionary<Type, string>();
		private readonly Dictionary<string, Type> _entitiesTypes = new Dictionary<string, Type>();
		private readonly IFileSystemManager _fileSystemManager;
		private readonly IServicesManager _servicesManager;
		private INoSqlConnector _noSqlConnector;

		private readonly ILogger _logger;
		//private LiteDatabase _liteDatabase;


		public EventDatabaseService(ILogger<EventDatabaseService> logger, IFileSystemManager fileSystemManager,
			NeonConfig config, IServicesManager servicesManager)
		{
			_logger = logger;
			_servicesManager = servicesManager;
			_fileSystemManager = fileSystemManager;
			_config = config;
		}

		public Task<bool> Start()
		{
			_logger.LogInformation($"Initializing Events Database, connector {_config.EventsDatabase.ConnectorName}");

			LoadConnector();
			ScanEntities();
			return Task.FromResult(true);
		}

		private void LoadConnector()
		{
			var connectorType = NoSqlUtils.GetNoSqlConnector(_config.EventsDatabase.ConnectorName);

			if (connectorType == null)
				throw new Exception($"NoSQL connector named {_config.EventsDatabase.ConnectorName} not found! ");

			_noSqlConnector = (INoSqlConnector)_servicesManager.Resolve(connectorType);

			_noSqlConnector.Init(_config.EventsDatabase.ConnectionString);

		}


		public Task<bool> Stop()
		{

			return Task.FromResult(true);
		}

		public T Insert<T>(T value) where T : IIotEntity
		{
			var collectionName = typeof(T).GetCustomAttribute<EventDatabaseEntityAttribute>().CollectionName;
			return _noSqlConnector.Insert(value, collectionName);
		}

		public T Update<T>(T value) where T : IIotEntity
		{

			var collectionName = typeof(T).GetCustomAttribute<EventDatabaseEntityAttribute>().CollectionName;
			return _noSqlConnector.Update(value, collectionName);
		}

		public List<T> List<T>() where T : IIotEntity
		{
			var collectionName = typeof(T).GetCustomAttribute<EventDatabaseEntityAttribute>().CollectionName;

			return _noSqlConnector.List<T>(collectionName);
		}

		public List<object> List(Type type)
		{
			var collectionName = type.GetCustomAttribute<EventDatabaseEntityAttribute>().CollectionName;
			//return ConvertCollection(collectionName, _liteDatabase.GetCollection(collectionName).FindAll().ToList());
			return new List<object>();
		}

		public Dictionary<string, List<object>> GetAllEvents()
		{
			var dict = new Dictionary<string, List<object>>();
			foreach (var entity in _entities)
			{
				var attr = entity.Key.GetCustomAttribute<EventDatabaseEntityAttribute>();


				dict.Add(attr.CollectionName, new List<object>());
				dict[attr.CollectionName] = List(entity.Key);
			}

			return dict;
		}


		public List<object> GetEventsByName(string collection)
		{
			var type = _entitiesTypes[collection];
			var openCast = _noSqlConnector.GetType().GetMethod(nameof(_noSqlConnector.Query));
			var closeCast = openCast.MakeGenericMethod(type);
			var queryable = closeCast.Invoke(_noSqlConnector, new object[] { collection }) as IQueryable<object>;

			return queryable.ToList();
		}


		public List<string> GetEventsName()
		{
			return _entities.Select(s => s.Key.GetCustomAttribute<EventDatabaseEntityAttribute>().CollectionName)
				.ToList();
		}

		public void ScanEntities()
		{
			_logger.LogInformation("Scan Entities");
			AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(EventDatabaseEntityAttribute)).ForEach(t =>
			{
				var attr = t.GetCustomAttribute<EventDatabaseEntityAttribute>();

				_entities.Add(t, attr.CollectionName);
				_entitiesTypes.Add(attr.CollectionName, t);

				if (!_noSqlConnector.CollectionExists(attr.CollectionName))
					foreach (var entry in GetEntitiesIndexes(t))
						_noSqlConnector.AddIndex(attr.CollectionName, entry.Key, entry.Value.Unique);
			});
		}

		public Dictionary<string, EventDatabaseIndexAttribute> GetEntitiesIndexes(Type type)
		{
			var dict = new Dictionary<string, EventDatabaseIndexAttribute>();

			type.GetProperties().ToList().ForEach(p =>
			{
				var attr = p.GetCustomAttribute<EventDatabaseIndexAttribute>();
				if (attr != null)
					dict.Add(p.Name, attr);
			});

			return dict;
		}

		private object ConvertCollection(string collectionName, BsonDocument document)
		{
			return BsonMapper.Global.ToObject(_entitiesTypes[collectionName], document);
		}

		private List<object> ConvertCollection(string collectionName, List<BsonDocument> documents)
		{
			return documents.Select(d => ConvertCollection(collectionName, d)).ToList();
		}
	}
}