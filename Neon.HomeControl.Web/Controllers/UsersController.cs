using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Impl.Controllers;
using Neon.HomeControl.Api.Core.Interfaces;
using Neon.HomeControl.Api.Core.Interfaces.Dao;
using Neon.HomeControl.Dto.Dto;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class UsersController : AbstractApiController<UserEntity, UserDto>
	{
		public UsersController(ILogger<UsersController> logger, IDataAccess<UserEntity> dataAccess,
			IDtoMapper<UserEntity, UserDto> mapper) : base(logger, dataAccess, mapper)
		{
		}
	}
}