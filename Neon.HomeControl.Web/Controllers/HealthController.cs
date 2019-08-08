using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Neon.HomeControl.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class HealthController
	{
		[HttpGet]
		public string Ping()
		{
			return "OK";
		}
	}
}
