using System;
using System.Collections.Generic;
using System.Linq;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Data.Services;
using Neon.HomeControl.Api.Core.Interfaces.Managers;

namespace Neon.HomeControl.StandardScriptLibrary.Services
{

	/// <summary>
	/// Class for control Services Manager
	/// </summary>
	[ScriptObject]
	public class ServiceManagerScriptObject
	{
		private readonly IServicesManager _servicesManager;

		public ServiceManagerScriptObject(IServicesManager servicesManager)
		{
			_servicesManager = servicesManager;
		}

		/// <summary>
		/// Resolve a service
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[ScriptFunction("SERVICES", "service_resolve", "Resolve service")]
		public object ResolveObject(Type type)
		{
			return _servicesManager.Resolve(type);

		}

		/// <summary>
		/// Get all services info
		/// </summary>
		/// <returns></returns>
		[ScriptFunction("SERVICES", "get_services_info", "Get all services info")]
		public List<ServiceInfo> GetServiceInfo()
		{
			return _servicesManager.ServicesInfo.ToList();
		}
	}
}
