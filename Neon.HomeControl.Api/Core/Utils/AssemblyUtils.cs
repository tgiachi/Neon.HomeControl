using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neon.HomeControl.Api.Core.Utils
{
	/// <summary>
	///     Class for manage assemblies
	/// </summary>
	public class AssemblyUtils
	{
		private static readonly List<Assembly> _assembliesCache = new List<Assembly>();

		/// <summary>
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="attributeType"></param>
		/// <returns></returns>
		public static List<Type> GetTypesWithCustomAttribute(Assembly assembly, Type attributeType)
		{
			return assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Length > 0).ToList();
		}


		public static Type GetInterfaceOfType(Type type)
		{
			try
			{
				return type.GetInterfaces()[0];
			}
			catch
			{
				return null;
			}
		}


		/// <summary>
		///     Restituisce il tipo controllando tutti gli assembly
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static Type GetType(string typeName)
		{
			var type = Type.GetType(typeName);
			if (type != null) return type;

			foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
			{
				type = a.GetType(typeName);
				if (type != null)
					return type;
			}

			foreach (var a in _assembliesCache)
			{
				type = a.GetType(typeName);
				if (type != null)
					return type;
			}

			return null;
		}

		/// <summary>
		///     Controlla tutti gli se implementano una interfaccia
		/// </summary>
		/// <param name="customInterface"></param>
		/// <returns></returns>
		public static List<Type> GetTypesImplentInterface(Type customInterface)
		{
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(customInterface.IsAssignableFrom);

			return types.ToList();
		}

		/// <summary>
		///     Prende tutt gli assembly (*.dll) da un attributo
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static List<Type> ScanAllAssembliesFromAttribute(Type attribute)
		{
			return ScanAssembly(attribute);
		}

		/// <summary>
		///     Prende gli assembly dell'applicazione
		/// </summary>
		/// <returns></returns>
		public static List<Assembly> GetAppAssemblies()
		{
			BuildAssemblyCache();
			return _assembliesCache;
		}

		public static Assembly[] GetAppAssembliesArray()
		{
			BuildAssemblyCache();

			return _assembliesCache.ToArray();
		}

		private static void BuildAssemblyCache()
		{
			if (_assembliesCache.Count == 0)
			{
				var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(s => !s.IsDynamic).ToList();
				var codeBase = Assembly.GetExecutingAssembly().CodeBase;
				var uri = new UriBuilder(codeBase);
				var path2 = Uri.UnescapeDataString(uri.Path);
				var path = Path.GetDirectoryName(path2);

				var files = Directory.GetFiles(path, "*.dll");
				foreach (var file in files)
					try
					{
						var existsAssembly = (from s in allAssemblies where s.Location.Contains(file) select s)
							.FirstOrDefault();

						if (existsAssembly == null)
						{
							var assembly = Assembly.LoadFile(file);
							allAssemblies.Add(assembly);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}

				_assembliesCache.AddRange(allAssemblies);

			}

		}

		/// <summary>
		///     Controlla tutti gli assembly se hanno l'attributo
		/// </summary>
		/// <param name="attribute"></param>
		/// <param name="filter"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static List<Type> ScanAssembly(Type attribute, string filter = "*.dll", string path = null)
		{
			var result = new List<Type>();

			// InitAppDomain();

			BuildAssemblyCache();

			foreach (var assembly in _assembliesCache)
			{
				try
				{
					result.AddRange(GetTypesWithCustomAttribute(assembly, attribute));
				}
				catch (Exception ex)
				{
					//Console.WriteLine(ex);
				}
			}



			return result;
		}

		/// <summary>
		///     Prende gli attributi di un tipo
		/// </summary>
		/// <param name="type"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static List<string> GetPropertiesFromAttribute(Type type, Type attribute)
		{
			var _dict = new List<string>();
			var props = type.GetProperties();
			foreach (var prop in props)
			{
				var attrs = prop.GetCustomAttributes(true);
				foreach (var attr in attrs)
					if (attr.GetType() == attribute)
						_dict.Add(prop.Name);
			}

			return _dict;
		}

		/// <summary>
		///     Prende la versione
		/// </summary>
		/// <returns></returns>
		public static string GetVersion()
		{
			var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
			return fvi.ProductVersion;
		}

		/// <summary>
		///     Prende il nome del prodotto
		/// </summary>
		/// <returns></returns>
		public static string GetProductName()
		{
			var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
			return fvi.ProductName;
		}

		/// <summary>
		///     Aggiunge un assembly alla cache
		/// </summary>
		/// <param name="assembly"></param>
		public static void AddAssemblyToCache(Assembly assembly)
		{
			//if (_assembliesCache.Count == 0)
			//{
			//	var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(s => !s.IsDynamic).ToList();
			//	_assembliesCache.AddRange(allAssemblies);
			//}

			BuildAssemblyCache();

			if (!_assembliesCache.Contains(assembly))
				_assembliesCache.Add(assembly);
		}


		private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			return Assembly.LoadFrom(args.Name);
		}


		/// <summary>
		///     Se debug
		/// </summary>
		/// <returns></returns>
		public static bool IsDebug()
		{
#if DEBUG
			return true;

#else
            return false;
#endif
		}
	}
}