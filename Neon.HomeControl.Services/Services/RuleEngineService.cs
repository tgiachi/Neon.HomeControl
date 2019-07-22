using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Rules;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using NLua;
using NReco.Linq;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IRuleEngineService), LoadAtStartup = true, Name = "Rule Engine service", Order = 99)]
	public class RuleEngineService : IRuleEngineService
	{

		private readonly ILogger _logger;
		private readonly ITaskExecutorService _taskExecutorService;
		private readonly IIoTService _ioTService;

		public ObservableCollection<RuleInfo> Rules { get; }


		public RuleEngineService(ILogger<IRuleEngineService> logger, IIoTService ioTService, ITaskExecutorService taskExecutorService)
		{
			_logger = logger;
			_ioTService = ioTService;
			_taskExecutorService = taskExecutorService;
			Rules = new ObservableCollection<RuleInfo>();

		}
		public Task<bool> Start()
		{
			_ioTService.GetEventStream<IIotEntity>().Subscribe(entity =>
				{
					_taskExecutorService.Enqueue(() =>
					{
						ParseRule(entity);
						return Task.CompletedTask;
					});
				});

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		private void ParseRule(IIotEntity entity)
		{
			Rules.Where(r => r.EntityType == entity.GetType()).ToList().ForEach(r =>
			{
				switch (r.RuleType)
				{
					case RuleTypeEnum.CSharp:
						ExecuteCSharpRule(r, entity);
						break;
					case RuleTypeEnum.Lambda:
						ExecuteLuaRule(r, entity);
						break;
					case RuleTypeEnum.Lua:
						ExecuteLuaRule(r, entity);
						break;
				}

			});
		}

		private void ExecuteLuaRule(RuleInfo rule, IIotEntity entity)
		{
			var lParser = new LambdaParser();
			var context = new Dictionary<string, object> { { "entity", entity }, { "rule", rule } };


			var bResult = (bool)lParser.Eval((string)rule.RuleCondition, context);

			if (bResult)
			{
				_logger.LogInformation($"Executing rule {rule.RuleName}");
				rule.Action.Invoke(entity);
			}
		}

		private void ExecuteCSharpRule(RuleInfo rule, IIotEntity entity)
		{
			var func = (Func<IIotEntity, bool>)rule.RuleCondition;

			if (func.Invoke(entity))
			{
				rule.Action.Invoke(entity);
			}
		}

		public void AddRule(string ruleName, Type entityType, string condition, Action<IIotEntity> action)
		{
			Rules.Add(new RuleInfo()
			{
				IsEnabled = true,
				EntityType = entityType,
				Action = action,
				RuleCondition = condition,
				RuleName = ruleName,
				RuleType = RuleTypeEnum.Lambda
			});
		}

		public void AddRule(string ruleName, Type entityType, Func<IIotEntity, bool> condition, Action<IIotEntity> action)
		{
			Rules.Add(new RuleInfo()
			{
				IsEnabled = true,
				EntityType = entityType,
				Action = action,
				RuleCondition = condition,
				RuleName = ruleName,
				RuleType = RuleTypeEnum.CSharp
			});
		}

		public void AddRule(string ruleName, Type entityType, string condition, LuaFunction action)
		{
			Rules.Add(new RuleInfo()
			{
				IsEnabled = true,
				EntityType = entityType,
				Action = entity => { action.Call(entity); },
				RuleCondition = condition,
				RuleName = ruleName,
				RuleType = RuleTypeEnum.Lua
			});
		}

	}
}
