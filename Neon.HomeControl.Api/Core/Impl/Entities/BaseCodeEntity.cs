using Neon.HomeControl.Api.Core.Interfaces.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neon.HomeControl.Api.Core.Impl.Entities
{
	public class BaseCodeEntity : BaseEntity, IBaseCodeEntity
	{
		[Key] [Column("code")] public Guid Code { get; set; }
	}
}