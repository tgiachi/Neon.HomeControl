namespace Neon.HomeControl.Api.Core.Enums
{
	/// <summary>
	///     Enum for choose polling timer
	/// </summary>
	public enum SchedulerServicePollingEnum
	{
		SHORT_POLLING = 10,
		NORMAL_POLLING = 60,
		LONG_POLLING = 300,
		VERY_LONG_POLLING = 3600
	}
}