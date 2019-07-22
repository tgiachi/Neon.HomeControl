using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Neon.HomeControl.Api.Core.Interfaces.Components;

namespace Neon.HomeControl.Api.Core.Data.Commands
{
	public class IotCommandInfo
	{
		public Type EntityType { get; set; }
		public string CommandName { get; set; }
		public IComponent Component { get; set; }
		public MethodInfo Method { get; set; }
	}
}
