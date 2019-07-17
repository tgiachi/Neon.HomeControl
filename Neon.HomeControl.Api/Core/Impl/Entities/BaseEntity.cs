using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Neon.HomeControl.Api.Core.Interfaces.Entities;

namespace Neon.HomeControl.Api.Core.Impl.Entities
{
	public class BaseEntity : IBaseEntity
	{
		
		[Key] 
		[Column("id")] 
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Column("created_date")] public DateTime CreatedDateTime { get; set; }

		[Column("updated_date")] public DateTime UpdateDateTime { get; set; }
	}
}