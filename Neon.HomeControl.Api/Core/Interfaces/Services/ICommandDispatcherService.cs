using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Data.Commands;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	/// <summary>
	/// Dispatcher Service
	/// </summary>
	public interface ICommandDispatcherService : IService
	{
		/// <summary>
		/// Dispatch to component service
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="commandName"></param>
		/// <param name="args"></param>
		object DispatchCommand<T>(T entity, string commandName, params object[] args) where T : IIotEntity;


		/// <summary>
		/// List of availables commands
		/// </summary>
		List<IotCommandInfo> CommandInfos { get; }
	}
}
