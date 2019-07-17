using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AutoMapper;
using Neon.HomeControl.Api.Core.Attributes.Dto;
using Neon.HomeControl.Api.Core.Utils;

namespace Neon.HomeControl.Api.Core.Mapper
{
	/// <summary>
	/// Default automapper scanning profile
	/// </summary>
	public class DefaultMapperProfile : Profile
	{
		/// <summary>
		/// ctor
		/// </summary>
		public DefaultMapperProfile()
		{
			var mappers = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(DtoMapAttribute));

			mappers.ForEach(t =>
			{
				var attr = t.GetCustomAttribute<DtoMapAttribute>();
				CreateMap(attr.EntityType, t).ReverseMap();
			});
		}
	}
}
