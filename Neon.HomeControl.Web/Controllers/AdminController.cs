using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Data.Components;
using Neon.HomeControl.Api.Core.Data.LuaScript;
using Neon.HomeControl.Api.Core.Data.Scheduler;
using Neon.HomeControl.Api.Core.Data.Services;
using Neon.HomeControl.Api.Core.Data.UserInteraction;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Neon.HomeControl.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AdminController : ControllerBase
	{
		private readonly IComponentsService _componentsService;
		private readonly ISchedulerService _schedulerService;
		private readonly IScriptService _scriptService;
		private readonly IServicesManager _servicesManager;
		private readonly IUserInteractionService _userInteractionService;

		public AdminController(IServicesManager servicesManager, IScriptService scriptService,
			IComponentsService componentsService, IUserInteractionService userInteractionService,
			ISchedulerService schedulerService)
		{
			_userInteractionService = userInteractionService;
			_servicesManager = servicesManager;
			_scriptService = scriptService;
			_componentsService = componentsService;
			_schedulerService = schedulerService;
		}

		[HttpGet]
		public ActionResult<List<ServiceInfo>> GetServices()
		{
			return Ok(_servicesManager.ServicesInfo);
		}

		[HttpGet]
		public ActionResult<List<JobInfo>> GetJobInfo()
		{
			return Ok(_schedulerService.JobsInfo);
		}

		[HttpGet]
		public ActionResult<List<LuaScriptFunctionData>> GetLuaGlobalFunctions()
		{
			return Ok(_scriptService.GlobalFunctions);
		}

		[HttpGet]
		public ActionResult<List<RunningComponentInfo>> GetRunningComponents()
		{
			return Ok(_componentsService.RunningComponents);
		}

		[HttpGet]
		public ActionResult<List<ComponentInfo>> GetAvaiableComponents()
		{
			return Ok(_componentsService.AvailableComponents);
		}

		[HttpGet]
		public ActionResult<List<UserInteractionData>> GetUserInteractionData()
		{
			return Ok(_userInteractionService.NeedUserInteractionData);
		}
	}
}