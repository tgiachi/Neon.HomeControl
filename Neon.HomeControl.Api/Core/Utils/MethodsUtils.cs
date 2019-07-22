using System;
using System.Linq;
using System.Reflection;
using System.Text;

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

		/// <summary>
		/// Return flatten Exception
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static string FlattenException(this Exception exception)
		{
			var stringBuilder = new StringBuilder();

			while (exception != null)
			{
				stringBuilder.AppendLine(exception.Message);
				stringBuilder.AppendLine(exception.StackTrace);

				exception = exception.InnerException;
			}

			return stringBuilder.ToString();
		}
	}
}