using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Neon.HomeControl.Api.Core.Events
{
	public class ScriptOutputEvent : INotification
	{
		public DateTime EventDate { get; set; }
		public string Category { get; set; }

		public string Level { get; set; }

		public string Message { get; set; }
	}
}
