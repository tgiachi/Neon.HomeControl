using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.IoT;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using LiteDB;
using MediatR;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Data.Commands;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;


namespace Neon.HomeControl.Services.Services
{
	/// <summary>
	///     Implementation of IoT Server
	/// </summary>
	[Service(typeof(IoTService), Name = "IoT Service", LoadAtStartup = true, Order = 2)]
	public class IoTService : IIoTService
	{
		private static readonly string _dbFilename = "Neon.HomeControl.IoT.db";
		private static readonly string _collectionName = "entities";
		private static LiteDatabase _liteDatabase;
		private readonly NeonConfig _config;
		private readonly IFileSystemService _fileSystemService;
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly INotificationService _notificationService;
		private readonly IMqttService _mqttService;
		private readonly Subject<IIotEntity> _iotEntitiesBus = new Subject<IIotEntity>();
		private readonly ICommandDispatcherService _commandDispatcherService;
		private readonly ILogger _logger;
		private readonly object _liteDbObjectLock = new object();


		/// <summary>
		///     Ctor
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="fileSystemService"></param>
		/// <param name="config"></param>
		public IoTService(ILogger<IIoTService> logger, IFileSystemService fileSystemService, 
			NeonConfig config, 
			IEventDatabaseService eventDatabaseService, 
			INotificationService notificationService,
			IMqttService mqttService,
			ICommandDispatcherService commandDispatcherService
		)
		{
			_logger = logger;
			_logger.LogError($"CREATED");

			_commandDispatcherService = commandDispatcherService;
			_mqttService = mqttService;
			_notificationService = notificationService;
			_eventDatabaseService = eventDatabaseService;
			_fileSystemService = fileSystemService;
			_config = config;
		}

		public Task<bool> Start()
		{
			_logger.LogInformation("Initializing IoT Database");
			_fileSystemService.CreateDirectory(_config.IoT.DatabaseDirectory);

			lock (_liteDbObjectLock)
			{
				_liteDatabase =
							new LiteDatabase(_fileSystemService.BuildFilePath(_config.IoT.DatabaseDirectory) + "\\" + _dbFilename);
				_liteDatabase.Shrink();
			}


			return Task.FromResult(true);
		}

	
		public T InsertEntity<T>(T value) where T : IIotEntity
		{
			lock (_liteDbObjectLock)
			{
				value.EntityType = typeof(T).FullName;

				_liteDatabase.GetCollection<T>(_collectionName).Insert(value);
				return value;
			}
		}

		public string GetEntityTypeByName(string name)
		{
			var entity = Query<BaseEventDatabaseEntity>().FirstOrDefault(d => d.EntityName == name);
				
			return entity?.EntityType;
		}

		public T GetEntityByType<T>(string name, string type) where T : IIotEntity
		{
			lock (_liteDbObjectLock)
			{
				return _liteDatabase.GetCollection<T>(_collectionName).FindOne(document =>
					document.EntityName == name && document.EntityType == type);
			}
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
			lock (_liteDbObjectLock)
			{
				value.EntityType = typeof(T).FullName;
				var updated = _liteDatabase.GetCollection<T>(_collectionName).Update(value);

				return value;
			}
		}

		public IQueryable<T> Query<T>() where T : IIotEntity
		{
			return _liteDatabase.GetCollection<T>(_collectionName).FindAll().AsQueryable();
		}

		public T InsertOrUpdate<T>(T value) where T : IIotEntity
		{
			if (value.Id == Guid.Empty)
				value.Id = Guid.NewGuid();

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
			_eventDatabaseService.Insert(value);
			InsertOrUpdate(value);

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