using System.Linq;
using System.Reflection;

namespace Neon.HomeControl.Api.Core.Utils
{
	public static class MethodsUtils
	{
		public static string[] GetMethodParamStrings(this ParameterInfo[] args)
		{
			var paramsInfos = new string[args.Length];
			var index = 0;
			args.ToList().ForEach(p =>
			{
				paramsInfos[index] = $"{p.Name} [{p.ParameterType.Name}]";
				index++;
			});

			return paramsInfos;
		}
	}
}