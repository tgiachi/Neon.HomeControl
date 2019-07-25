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
	public class RolesController : AbstractApiController<RoleEntity, RoleDto>
	{
		public RolesController(ILogger<RolesController> logger, IDataAccess<RoleEntity> dataAccess,
			IDtoMapper<RoleEntity, RoleDto> mapper) : base(logger, dataAccess, mapper)
		{
		}
	}
}