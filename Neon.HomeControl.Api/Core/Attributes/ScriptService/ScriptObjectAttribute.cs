using System;

namespace Neon.HomeControl.Api.Core.Attributes.ScriptService
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ScriptObjectAttribute : Attribute
	{
		/// <summary>
		/// Object name in script engine (ex: logger.log_info())
		/// </summary>
		public string ObjName { get; set; }

		public ScriptObjectAttribute(string objName)
		{
			ObjName = objName;
		}
	}
}