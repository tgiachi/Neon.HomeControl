using System.Linq;
using System.Reflection;
using Neon.HomeControl.Api.Core.Attributes.OAuth;
using Neon.HomeControl.Api.Core.Data.OAuth;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.OAuth;
using Neon.HomeControl.Api.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Neon.HomeControl.Web.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class OAuthController : ControllerBase
	{
		private readonly IServicesManager _servicesManager;

		public OAuthController(IServicesManager servicesManager)
		{
			_servicesManager = servicesManager;
		}


		[HttpGet("{provider}/")]
		public ActionResult<bool> Authorize(string provider)
		{
			var type = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(OAuthProviderAttribute)).ToList()
				.FirstOrDefault(t =>
				{
					var attr = t.GetCustomAttribute<OAuthProviderAttribute>();

					return attr.Name.ToLower() == provider;
				});

			//	type = AssemblyUtils.GetInterfaceOfType(type);
			var oauthResult = new OAuthResult();

			if (!string.IsNullOrEmpty(HttpContext.Request.Query["code"]))
				oauthResult.Code = HttpContext.Request.Query["code"];

			if (!string.IsNullOrEmpty(HttpContext.Request.Query["token"]))
				oauthResult.Code = HttpContext.Request.Query["token"];

			if (!string.IsNullOrEmpty(HttpContext.Request.Query["status"]))
				oauthResult.Code = HttpContext.Request.Query["status"];

			if (type != null)
			{
				var callback = _servicesManager.Resolve(type) as IOAuthCallback;

				callback?.OnOAuthResult(oauthResult);
			}

			return NotFound(true);
		}
	}
}