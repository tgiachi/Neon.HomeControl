using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Components.Components;
using Neon.HomeControl.Components.Interfaces;

namespace Neon.HomeControl.Components.Controllers
{
	[ApiController]
	[Route("api/components/plexhook/")]
	public class PlexHookApiController : ControllerBase
	{
		private readonly IServicesManager _servicesManager;

		public PlexHookApiController(IServicesManager servicesManager)
		{
			_servicesManager = servicesManager;
		}

		[HttpPost]
		public string Hook(string jsonData)
		{
			_servicesManager.Resolve<IPlexHookComponent>().Hook(jsonData);

			return "ok";
		}
	}
}
