using Neon.HomeControl.Api.Core.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Neon.HomeControl.Api.Core.Data.Commands
{
	public class IotCommandInfo
	{
		public Type EntityType { get; set; }
		public string CommandName { get; set; }

		[JsonIgnore]
		public IComponent Component { get; set; }
		[JsonIgnore]
		public MethodInfo Method { get; set; }

		public string MethodName { get; set; }
		public List<IotCommandParamInfo> Params { get; set; }

		public IotCommandInfo()
		{
			Params = new List<IotCommandParamInfo>();
		}
	}
}
