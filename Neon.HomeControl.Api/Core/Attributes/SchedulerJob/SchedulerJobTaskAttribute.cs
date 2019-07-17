using System;

namespace Neon.HomeControl.Api.Core.Attributes.SchedulerJob
{
	/// <summary>
	///     Attribute for creating scheduled actions
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class SchedulerJobTaskAttribute : Attribute
	{
		/// <summary>
		///     Attribute for creating scheduled actions
		/// </summary>
		/// <param name="startNow">If True, of starts execute job</param>
		/// <param name="seconds">Number of seconds</param>
		public SchedulerJobTaskAttribute(bool startNow, int seconds)
		{
			StartNow = startNow;
			Seconds = seconds;
		}

		/// <summary>
		///     If True, of starts execute job
		/// </summary>
		public bool StartNow { get; set; }

		/// <summary>
		///     Number of secondse
		/// </summary>
		public int Seconds { get; set; }
	}
}