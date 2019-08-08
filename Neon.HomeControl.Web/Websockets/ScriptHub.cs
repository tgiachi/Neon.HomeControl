using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Neon.HomeControl.Api.Core.Attributes.WebSocket;
using Neon.HomeControl.Api.Core.Events;
using Neon.HomeControl.Api.Core.Utils;
using WebSocketManager;
using WebSocketManager.Common;

namespace Neon.HomeControl.Web.Websockets
{

	[WebSocketHub("/ws/scripts")]
	public class ScriptHub : WebSocketHandler, INotificationHandler<ScriptOutputEvent>
	{
		public ScriptHub(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
		{
		}

		public async Task Handle(ScriptOutputEvent notification, CancellationToken cancellationToken)
		{
			await SendMessageToAllAsync(new Message()
			{
				MessageType = MessageType.Text,
				Data = notification.ToJson()
			});
		}
	}
}
