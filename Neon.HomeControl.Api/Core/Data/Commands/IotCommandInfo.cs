using Neon.HomeControl.Api.Core.Interfaces.Components;
using System;
using System.Reflection;

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
