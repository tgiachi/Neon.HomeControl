using AutoMapper;
using Neon.HomeControl.Api.Core.Attributes.Dto;
using Neon.HomeControl.Api.Core.Utils;
using System.Reflection;

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
