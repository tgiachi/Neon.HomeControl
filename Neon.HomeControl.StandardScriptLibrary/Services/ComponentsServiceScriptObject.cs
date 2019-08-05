using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Interfaces.Managers;

namespace Neon.HomeControl.StandardScriptLibrary.Services
{
	/// <summary>
	/// Bridge for control components
	/// </summary>
	[ScriptObject]
	public class ComponentsServiceScriptObject
	{

		private readonly ILogger _logger;
		private readonly IComponentsService _componentsService;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="componentsService"></param>
		public ComponentsServiceScriptObject(ILogger<ComponentsServiceScriptObject> logger,
			IComponentsService componentsService)
		{
			_logger = logger;
			_componentsService = componentsService;
		}

		[ScriptFunction("COMPONENTS", "load_component", "Load component")]
		public bool LoadComponent(string componentName)
		{
			_logger.LogInformation($"Loading service name {componentName}");

			_componentsService.StartComponent(componentName).ContinueWith(task => task.Result);

			return true;

		}
	}
}
