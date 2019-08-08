using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Neon.HomeControl.Api.Core.Attributes.WebSocket;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Services.Services;
using WebSocketManager;
using WebSocketManager.Common;

namespace Neon.HomeControl.Web.Websockets
{

	[WebSocketHub("/ws/events")]
	public class EventsHub : WebSocketHandler
	{
		
		public EventsHub(WebSocketConnectionManager webSocketConnectionManager, IIoTService ioTService) : base(webSocketConnectionManager)
		{
			ioTService.GetEventStream<IIotEntity>().Subscribe(async entity =>
			{
				await SendMessageToAllAsync(new Message()
				{
					MessageType = MessageType.Text,
					Data = entity.ToJson()
				});
			});
		}
	}
}
