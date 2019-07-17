using System;
using Newtonsoft.Json;

namespace Neon.HomeControl.Api.Core.Utils
{
	/// <summary>
	///     Extension class for Serialize/Deserialize JSON
	/// </summary>
	public static class JsonUtils
	{
		/// <summary>
		///     Serialize object to string
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToJson(this object value)
		{
			return JsonConvert.SerializeObject(value, Formatting.Indented);
		}

		/// <summary>
		///     Parse string to Type
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object FromJson(this string obj, Type type)
		{
			try
			{
				return JsonConvert.DeserializeObject(obj, type);
			}
			catch (Exception)
			{
				Console.WriteLine($"Can't convert {obj} to object {type.Name}");
				return null;
			}
		}

		/// <summary>
		///     Parse string to Generic
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T FromJson<T>(this string obj)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(obj);
			}
			catch (Exception)
			{
				Console.WriteLine($"Can't convert {obj} to object {typeof(T).Name}");
				return default;
			}
		}
	}
}