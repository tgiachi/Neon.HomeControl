using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	/// <summary>
	/// Database wrapper for DB Context
	/// </summary>
	public interface IDatabaseService : IService
	{
		/// <summary>
		/// Type of DB Context
		/// </summary>
		Type GetDbContextForContainer { get; set; }
	}
}