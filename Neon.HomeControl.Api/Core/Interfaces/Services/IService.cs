﻿using Neon.HomeControl.Api.Core.Interfaces.Managers;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	/// <summary>
	///     Interface for create System Services
	/// </summary>
	public interface IService
	{
		/// <summary>
		///     Start service from <see cref="IServicesManager" />
		/// </summary>
		/// <returns></returns>
		Task<bool> Start();

		/// <summary>
		///     Stop service from <see cref="IServicesManager" />
		/// </summary>
		/// <returns></returns>
		Task<bool> Stop();
	}
}