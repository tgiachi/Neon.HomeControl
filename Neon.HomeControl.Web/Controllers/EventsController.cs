using Microsoft.AspNetCore.Mvc;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using System.Collections.Generic;

namespace Neon.HomeControl.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class EventsController : ControllerBase
	{
		private readonly IEventDatabaseService _eventDatabaseService;

		public EventsController(IEventDatabaseService eventDatabaseService)
		{
			_eventDatabaseService = eventDatabaseService;
		}


		[HttpGet]
		public ActionResult<List<string>> GetEventsName()
		{
			return Ok(_eventDatabaseService.GetEventsName());
		}

		[HttpGet("/Events/{name}")]
		public ActionResult<List<object>> GetEventData(string name)
		{
			var events = _eventDatabaseService.GetEventsByName(name);

			if (events != null)
				return Ok(events);
			return NotFound();
		}
	}
}