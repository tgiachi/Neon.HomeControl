using Microsoft.AspNetCore.Mvc;
using Neon.HomeControl.Web.Auth;

namespace Neon.HomeControl.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AuthController : ControllerBase
	{
		private readonly AuthenticationManager _authenticationManager;

		public AuthController(AuthenticationManager authenticationManager)
		{
			_authenticationManager = authenticationManager;
		}


		[HttpPost]
		public ActionResult Login(string email, string password)
		{
			var token = _authenticationManager.Authenticate(email, password);

			if (token == null)
				return BadRequest("Username or password incorrect");

			return Ok(token);
		}
	}
}