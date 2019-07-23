using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Neon.HomeControl.Api.Core.Events.System
{
	/// <summary>
	/// System event triggered when services manager loaded all service
	/// </summary>
	public class ServiceLoadedEvent  : INotification
	{

	}
}
