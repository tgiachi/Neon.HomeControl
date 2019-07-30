using LiteDB;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.IoT;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Interfaces.Database;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Utils;


namespace Neon.HomeControl.Services.Services
{
	/// <summary>
	///     Implementation of IoT Server
	/// </summary>
	[Service(typeof(IIoTService), Name = "IoT Service", LoadAtStartup = true, Order = 2)]
	public class IoTService : IIoTService
	{
		private static readonly string _dbFilename = "Neon.HomeControl.IoT.db";
		private static readonly string _collectionName = "entities";
		private readonly NeonConfig _config;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly IMqttService _mqttService;
		private readonly IServicesManager _servicesManager;
		private readonly Subject<IIotEntity> _iotEntitiesBus = new Subject<IIotEntity>();
		private readonly ILogger _logger;
		private INoSqlConnector _noSqlConnector;


		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="fileSystemManager"></param>
		/// <param name="config"></param>
		/// <param name="eventDatabaseService"></param>
		/// <param name="servicesManager"></param>
		/// <param name="mqttService"></param>
		public IoTService(ILogger<IIoTService> logger, IFileSystemManager fileSystemManager,
			NeonConfig config,
			IEventDatabaseService eventDatabaseService,
			IServicesManager servicesManager,
				IMqttService mqttService
		)
		{
			_logger = logger;
			_mqttService = mqttService;
			_eventDatabaseService = eventDatabaseService;
			_fileSystemManager = fileSystemManager;
			_servicesManager = servicesManager;
			_config = config;
		}

		public Task<bool> Start()
		{
			_logger.LogInformation("Initializing IoT Database");

			LoadConnector();

			return Task.FromResult(true);
		}

		private void LoadConnector()
		{
			var connectorType = NoSqlUtils.GetNoSqlConnector(_config.IoT.ConnectorName);

			if (connectorType == null)
				throw new Exception($"NoSQL connector named {_config.IoT.ConnectorName} not found! ");

			_noSqlConnector = (INoSqlConnector)_servicesManager.Resolve(connectorType);

			_noSqlConnector.Init(_config.IoT.ConnectionString);

		}


		public T InsertEntity<T>(T value) where T : IIotEntity
		{
			
				value.EntityType = typeof(T).FullName;

				_noSqlConnector.Insert(value, _collectionName);
				return value;
			
		}

		public string GetEntityTypeByName(string name)
		{
			var entity = Query<BaseEventDatabaseEntity>().FirstOrDefault(d => d.EntityName == name);

			return entity?.EntityType;
		}

		public T GetEntityByType<T>(string name, string type) where T : IIotEntity
		{
			return _noSqlConnector.Query<T>(_collectionName).FirstOrDefault(document =>
					document.EntityName == name && document.EntityType == type);
		}

		/// <summary>
		/// Get all entities
		/// </summary>
		/// <returns></returns>
		public List<IIotEntity> GetEntities()
		{
			return Query<IIotEntity>().ToList();
		}

		public T Update<T>(T value) where T : IIotEntity
		{

			_noSqlConnector.Update(value, _collectionName);

				return value;
			
		}

		public IQueryable<T> Query<T>() where T : IIotEntity
		{
			return _noSqlConnector.Query<T>(_collectionName);
		}

		public T InsertOrUpdate<T>(T value) where T : IIotEntity
		{
			if (value.Id == Guid.Empty)
				value.Id = Guid.NewGuid();

			if (string.IsNullOrEmpty(value.EntityType))
				value.EntityType = value.GetType().FullName;

			T obj = default;
			var returnObj = default(T);

			if (string.IsNullOrEmpty(value.EntityName))
				obj = Query<T>().FirstOrDefault(e => e.EntityType == typeof(T).FullName);
			else
				obj = Query<T>().FirstOrDefault(e => e.EntityName == value.EntityName);


			if (obj == null)
			{
				returnObj = InsertEntity(value);
			}
			else
			{
				value.Id = obj.Id;
				returnObj = Update(value);
			}

			if (EntityHaveChanges(obj, value))
				Publish(value);

			return returnObj;
		}

		public T FindById<T>(Guid id) where T : IIotEntity
		{
			return Query<T>().FirstOrDefault(s => s.Id == id);
		}

		public IObservable<T> GetEventStream<T>() where T : IIotEntity
		{
			return _iotEntitiesBus.OfType<T>();
		}

		public T InsertEvent<T>(T value) where T : IIotEntity
		{

			InsertOrUpdate(value);
			_eventDatabaseService.Insert(value);


			_mqttService.SendMessage($"events/{value.EntityType}", value);

			return value;
		}

		public void Publish<T>(T @event) where T : IIotEntity
		{
			_logger.LogDebug($"Publishing changes of {@event.GetType().Name}/{@event.EntityName}");
			_iotEntitiesBus.OnNext(@event);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		private static bool EntityHaveChanges(object oldEntity, object newEntity)
		{
			if (oldEntity == null)
				return true;

			var properties = oldEntity.GetType().GetProperties();

			foreach (var pi in properties)
			{
				if (pi.CustomAttributes.Any(ca => ca.AttributeType == typeof(IgnorePropertyCompareAttribute))) continue;

				object oldValue = pi.GetValue(oldEntity), newValue = pi.GetValue(newEntity);

				if (!Equals(oldValue, newValue)) return true;
			}

			return false;
		}
	}
}