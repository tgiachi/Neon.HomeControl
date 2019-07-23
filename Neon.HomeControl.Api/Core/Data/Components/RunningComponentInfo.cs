using System;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Components;
using Newtonsoft.Json;

namespace Neon.HomeControl.Api.Core.Data.Components
{
	public class RunningComponentInfo : ComponentInfo
	{
		[JsonIgnore] public IComponent Component { get; set; }

		public ComponentStatusEnum Status { get; set; }
		public Exception Error { get; set; }
	}
}