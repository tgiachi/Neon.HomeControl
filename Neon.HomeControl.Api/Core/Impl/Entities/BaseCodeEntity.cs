using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Neon.HomeControl.Api.Core.Interfaces.Entities;

namespace Neon.HomeControl.Api.Core.Impl.Entities
{
	public class BaseCodeEntity : BaseEntity, IBaseCodeEntity
	{
		[Key] [Column("code")] public Guid Code { get; set; }
	}
}