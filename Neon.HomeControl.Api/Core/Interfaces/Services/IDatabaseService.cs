using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IDatabaseService : IService
	{
		Type GetDbContextForContainer { get; set; }
	}
}