using System;
using System.Collections.ObjectModel;
using Neon.HomeControl.Api.Core.Data.Rules;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using NLua;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{

	/// <summary>
	/// Rules engine service
	/// </summary>
	public interface IRuleEngineService : IService
	{
		/// <summary>
		/// Add rule with string condition with Lambda parser
		/// </summary>
		/// <param name="ruleName"></param>
		/// <param name="condition"></param>
		/// <param name="action"></param>
		void AddRule(string ruleName, Type entityType, string condition, Action<IIotEntity> action);

		/// <summary>
		/// Add rule with function boolean 
		/// </summary>
		/// <param name="ruleName"></param>
		/// <param name="condition"></param>
		/// <param name="action"></param>
		void AddRule(string ruleName, Type entityType, Func<IIotEntity, bool> condition, Action<IIotEntity> action);

		/// <summary>
		/// Add rule with string condition for ScriptManager
		/// </summary>
		/// <param name="ruleName"></param>
		/// <param name="condition"></param>
		/// <param name="action"></param>
		void AddRule(string ruleName, Type entityType, string condition, LuaFunction action);

		/// <summary>
		/// Get all rules
		/// </summary>
		ObservableCollection<RuleInfo> Rules { get; }
	}
}
