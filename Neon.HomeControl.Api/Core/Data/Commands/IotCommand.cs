using MediatR;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;

namespace Neon.HomeControl.Api.Core.Data.Commands
{
	/// <summary>
	/// Command to send to devices
	/// </summary>
	public class IotCommand<T> : INotification where T : IIotEntity
	{
		/// <summary>
		/// Entity
		/// </summary>
		public T Entity { get; set; }
		/// <summary>
		/// Command name
		/// </summary>
		public string CommandName { get; set; }
		/// <summary>
		/// Command Parameters
		/// </summary>
		public object[] Parameters { get; set; }

		/// <summary>
		/// Helper for build command
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="commandName"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static IotCommand<T> BuildCommand<T>(T entity, string commandName, params object[] args) where T : IIotEntity
		{
			return new IotCommand<T>()
			{
				Entity = entity,
				CommandName = commandName,
				Parameters = args
			};
		}
	}
}
