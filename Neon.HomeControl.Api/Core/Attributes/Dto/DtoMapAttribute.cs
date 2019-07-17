using System;

namespace Neon.HomeControl.Api.Core.Attributes.Dto
{
	/// <summary>
	///     Attribute for mapping entities and DTOs
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class DtoMapAttribute : Attribute
	{
		/// <summary>
		///     DB Entity
		/// </summary>
		/// <param name="entityType"></param>
		public DtoMapAttribute(Type entityType)
		{
			EntityType = entityType;
		}

		/// <summary>
		///     DB Entity
		/// </summary>
		public Type EntityType { get; set; }
	}
}