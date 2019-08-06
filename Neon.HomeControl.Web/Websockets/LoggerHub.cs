using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Data.Logger;
using Neon.HomeControl.Api.Core.Utils;
using WebSocketManager;
using WebSocketManager.Common;

namespace Neon.HomeControl.Web.Websockets
{
	public class LoggerHub : WebSocketHandler
	{
		public LoggerHub(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
		{
			AppUtils.LoggerEntries.CollectionChanged +=async (sender, args) =>
			{
				foreach (var t in args.NewItems)
				{
					var logEntry = t as LoggerEntry;
					await SendMessageToAllAsync(new Message()
					{
						MessageType = MessageType.Text,
						Data = logEntry.ToJson()
					});
				}
			};
		}
	}
}
