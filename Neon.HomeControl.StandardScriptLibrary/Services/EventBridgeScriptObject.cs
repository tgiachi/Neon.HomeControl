using System;
using System.Collections.Generic;
using System.Linq;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using NLua;

namespace Neon.HomeControl.StandardScriptLibrary.Services
{

	/// <summary>
	/// Events manager for LUA
	/// </summary>
	[ScriptObject("events")]
	public class EventBridgeScriptObject
	{

		private readonly IIoTService _ioTService;
		private readonly ISchedulerService _schedulerService;
		private readonly ICommandDispatcherService _commandDispatcherService;
		private readonly IRuleEngineService _ruleEngineService;

		public EventBridgeScriptObject(IIoTService ioTService, ISchedulerService schedulerService, ICommandDispatcherService commandDispatcherService, IRuleEngineService ruleEngineService)
		{
			_ioTService = ioTService;
			_schedulerService = schedulerService;
			_commandDispatcherService = commandDispatcherService;
			_ruleEngineService = ruleEngineService;
		}


		/// <summary>
		/// Add new alarm
		/// </summary>
		/// <param name="name"></param>
		/// <param name="hours"></param>
		/// <param name="minutes"></param>
		/// <param name="function"></param>
		[ScriptFunction("TIMER", "add_alarm", "Add new alarm")]
		public void AddAlarm(string name, int hours, int minutes, LuaFunction function)
		{
			_schedulerService.AddJob(() => { function.Call(); }, name, hours, minutes);
		}



		/// <summary>
		/// Get all entities
		/// </summary>
		/// <returns></returns>
		[ScriptFunction("ENTITIES", "get_entities", "Get all entities")]
		public List<BaseEventDatabaseEntity> GetEntities()
		{
			return _ioTService.Query<BaseEventDatabaseEntity>().ToList();
		}

		/// <summary>
		/// Get Entity by Name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[ScriptFunction("ENTITIES", "get_entity_by_name", "Get entity passing name")]
		public BaseEventDatabaseEntity GetEntityByName(string name)
		{
			return _ioTService.Query<BaseEventDatabaseEntity>().FirstOrDefault(e => e.EntityName == name);
		}


		/// <summary>
		/// Transform entity to Generic Entity
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		[ScriptFunction("ENTITIES", "cast_entity", "Transform entity to Generic entity")]
		public T GetEntityType<T>(object entity) where T : class
		{
			var castedEntity = ((IIotEntity)entity);
			var type = castedEntity.EntityType;

			var openCast = _ioTService.GetType().GetMethod(nameof(_ioTService.GetEntityByType));
			var closeCast = openCast.MakeGenericMethod(AssemblyUtils.GetType(type));
			return (T)closeCast.Invoke(_ioTService, new object[] { castedEntity.EntityName, castedEntity.EntityType });
		}

		static T Cast<T>(object entity) where T : class
		{
			return entity as T;
		}


		/// <summary>
		/// Subscribe to event with name
		/// </summary>
		/// <param name="entityName"></param>
		/// <param name="function"></param>
		[ScriptFunction("EVENTS", "on_event_name", "Subscribe on event")]
		public void OnEventName(string entityName, LuaFunction function)
		{
			_ioTService.GetEventStream<IIotEntity>().Subscribe(entity =>
			{
				if (string.Equals(entity.EntityName, entityName, StringComparison.CurrentCultureIgnoreCase))
				{
					try
					{
						function.Call(entity);
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
			});
		}


		/// <summary>
		/// Send command to event system
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="commandName"></param>
		/// <param name="args"></param>
		[ScriptFunction("EVENTS", "send_command", "Send command ")]
		public object SendCommand(IIotEntity entity, string commandName, params object[] args)
		{
			var type = entity.EntityType;
			var openCast2 = _commandDispatcherService.GetType().GetMethod(nameof(_commandDispatcherService.DispatchCommand));
			var closeCase2 = openCast2.MakeGenericMethod(new[] { AssemblyUtils.GetType(type) });

			var cmd = closeCase2.Invoke(_commandDispatcherService, new object[] { entity, commandName, args });

			return cmd;

		}

		/// <summary>
		/// Subscribe to event with Type (without full class name)
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="function"></param>
		[ScriptFunction("RULES", "on_event_type", "Subscribe on event")]
		public void OnEventType(string eventType, LuaFunction function)
		{

			_ioTService.GetEventStream<IIotEntity>().Subscribe(entity =>
			{
				if (string.Equals(Type.GetType(entity.EntityType).Name, eventType, StringComparison.CurrentCultureIgnoreCase))
				{
					function.Call(entity);
				}
			});
		}

		[ScriptFunction("ENTITIES", "get_entity_type", "Subscribe on event")]
		public string GetEntityTypeByName(string entityName)
		{
			return _ioTService.GetEntityTypeByName(entityName);
		}


		[ScriptFunction("RULES", "add_rule", "Add rule to event")]
		public void AddRule(string ruleName, string entityName, string condition, LuaFunction function)
		{
			_ruleEngineService.AddRule(ruleName, AssemblyUtils.GetType(GetEntityTypeByName(entityName)), condition, function);
		}
	}
}
