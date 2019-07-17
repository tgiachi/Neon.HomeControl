using System;

namespace Neon.HomeControl.Api.Core.Enums
{
	/// <summary>
	///     Component status enum
	/// </summary>
	[Flags]
	public enum ComponentStatusEnum
	{
		/// <summary>
		///     Component is Stopped
		/// </summary>
		STOPPED,

		/// <summary>
		///     Component is Started
		/// </summary>
		STARTED,

		/// <summary>
		///     Component have configuration Error
		/// </summary>
		CONFIGURATION_ERROR,

		/// <summary>
		///     Component have generic error
		/// </summary>
		ERROR
	}
}