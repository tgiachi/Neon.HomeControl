using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Commands;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Commands;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Data.Components;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(ICommandDispatcherService), LoadAtStartup = true, Name = "Entity command dispatcher", Order = 99)]
	public class CommandDispatcherService : ICommandDispatcherService
	{
		private readonly IComponentsService _componentsService;
		private readonly INotificationService _notificationService;
		private readonly ILogger _logger;

		private List<IotCommandInfo> _commands = new List<IotCommandInfo>();

		/// <summary>
		/// Commands
		/// </summary>
		public List<IotCommandInfo> CommandInfos => _commands;

		public CommandDispatcherService(ILogger<ICommandDispatcherService> logger,
			IComponentsService componentsService,
			INotificationService notificationService)
		{
			_logger = logger;
			_notificationService = notificationService;
			_componentsService = componentsService;
			_componentsService.RunningComponents.CollectionChanged += (sender, s) =>
			{
				var oc = (ObservableCollection<RunningComponentInfo>)sender;
				//_commands.Clear();
				var readonlyList = s.NewItems[0] as RunningComponentInfo;

				ScanCommands(new List<RunningComponentInfo>() { readonlyList });
			};
		}


		/// <summary>
		/// Dispatch command to commands
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="commandName"></param>
		/// <param name="args"></param>
		public object DispatchCommand<T>(T entity, string commandName, params object[] args) where T : IIotEntity
		{

			_notificationService.BroadcastMessage(IotCommand<T>.BuildCommand(entity, commandName, args));

			var commandMethod = _commands
				.FirstOrDefault(s => s.EntityType == typeof(T) && s.CommandName == commandName.ToUpper());

			try
			{
				var result = commandMethod?.Method.Invoke(commandMethod.Component, new object[] { entity, commandName, args });

				return result;
			}
			catch (Exception e)
			{
				_logger.LogError($"Error during dispatch command {commandName} for Entity: {entity.GetType().Name} => {e}");

				return null;
			}
		}



		private void ScanCommands(List<RunningComponentInfo> components)
		{
			components.ForEach(
				c =>
				{
					c.Component?.GetType().GetMethods().ToList().ForEach(m =>
					{
						var commandAttributes = m.GetCustomAttribute<IotCommandAttribute>();

						if (commandAttributes == null) return;

						var commandParamsInfo = new List<IotCommandParamInfo>();

						var commandParams = m.GetCustomAttributes<IotCommandParamAttribute>();

						commandParams.ToList().ForEach(cmd =>
						{
							commandParamsInfo.Add(new IotCommandParamInfo()
							{
								ParamName = cmd.Name,
								IsRequired = cmd.IsRequired
							});
						});


						_commands.Add(new IotCommandInfo()
						{
							CommandName = commandAttributes.CommandName.ToUpper(),
							Method = m,
							MethodName = m.Name,
							Params = commandParamsInfo,
							EntityType = commandAttributes.EntityType,
							Component = c.Component
						});

						_logger.LogInformation($"Adding command {commandAttributes.CommandName} for component {c.Name}");
					});
				});

		}

		public Task<bool> Start()
		{
			ScanCommands(_componentsService.RunningComponents.ToList());
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}
