using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.ScriptEngine
{
	/// <summary>
	/// Interface for creates Universal Script Engine
	/// </summary>
	public interface IScriptEngine : IDisposable
	{
		void LoadFile(string fileName, bool immediateExecute);

		void RegisterFunction(string functionName, object obj, MethodInfo method);

		Task<bool> Build();

	}
}
