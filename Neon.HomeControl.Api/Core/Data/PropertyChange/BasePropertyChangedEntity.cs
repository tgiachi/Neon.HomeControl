using System.ComponentModel;
using Neon.HomeControl.Api.Core.Interfaces.PropertyChange;

namespace Neon.HomeControl.Api.Core.Data.PropertyChange
{
	/// <summary>
	///     Implementation of Property Change Entity
	/// </summary>
	public class BasePropertyChangedEntity : IBasePropertyChangedEntity
	{
		/// <summary>
		///     Event for track property Changed
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
	}
}