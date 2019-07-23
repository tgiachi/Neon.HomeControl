using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Commands;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Commands;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Components;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(ICommandDispatcherService), LoadAtStartup = true, Name = "Entity command dispatcher", Order = 99)]
	public class CommandDispatcherService : ICommandDispatcherService
	{
		private readonly IComponentsService _componentsService;
		private readonly INotificationService _notificationService;
		private readonly ILogger _logger;

		private List<IotCommandInfo> _commands = new List<IotCommandInfo>();

		public CommandDispatcherService(ILogger<ICommandDispatcherService> logger,
			IComponentsService componentsService, 
			INotificationService notificationService)
		{
			_logger = logger;
			_notificationService = notificationService;
			_componentsService = componentsService;
			_componentsService.RunningComponents.CollectionChanged += (sender, s) =>
			{
				_commands.Clear();
				ScanCommands();
			};
		}


		/// <summary>
		/// Dispatch command to commands
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="commandName"></param>
		/// <param name="args"></param>
		public void DispatchCommand<T>(T entity, string commandName, params object[] args) where T : IIotEntity
		{

			_notificationService.BroadcastMessage(IotCommand<T>.BuildCommand(entity, commandName, args));

			var commandMethod = _commands
				.FirstOrDefault(s => s.EntityType == typeof(T) && s.CommandName == commandName.ToUpper());

			try
			{
				commandMethod?.Method.Invoke(commandMethod.Component, new object[] { entity, commandName, args });
			}
			catch (Exception e)
			{
				_logger.LogError($"Error during dispatch command {commandName} for Entity: {entity.GetType().Name} => {e}");
			}

		}

		private void ScanCommands()
		{
			_componentsService.RunningComponents.Where(c => c.Status == ComponentStatusEnum.STARTED).ToList().ForEach(
				c =>
				{
					c.Component.GetType().GetMethods().ToList().ForEach(m =>
					{
						var commandAttributes = m.GetCustomAttribute<IotCommandAttribute>();

						if (commandAttributes == null) return;

						_commands.Add(new IotCommandInfo()
						{
							CommandName = commandAttributes.CommandName.ToUpper(),
							Method = m,
							EntityType = commandAttributes.EntityType,
							Component = c.Component
						});

						_logger.LogInformation($"Adding command {commandAttributes.CommandName} for component {c.Name}");
					});

				});

		}

		public Task<bool> Start()
		{
			ScanCommands();
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}
