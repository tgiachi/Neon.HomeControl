using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using System;

namespace Neon.HomeControl.Api.Core.Data.Rules
{
	public class RuleInfo
	{
		public string RuleName { get; set; }

		public object RuleCondition { get; set; }

		public Type EntityType { get; set; }
		public Action<IIotEntity> Action { get; set; }

		public RuleTypeEnum RuleType { get; set; }

		public bool IsEnabled { get; set; }
	}
}
