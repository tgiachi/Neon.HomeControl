using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IFileSystemService), LoadAtStartup = true, Order = 2)]
	public class FileSystemService : IFileSystemService
	{
		private static readonly string _pidFile = "Neon.home.pid";
		private readonly NeonConfig _config;

		private readonly ILogger _logger;

		public FileSystemService(NeonConfig config, ILogger<FileSystemService> logger)
		{
			_logger = logger;
			_config = config;
		}

		public string RootDirectory { get; set; }

		public Task<bool> Start()
		{
			_logger.LogInformation($"Root directory {_config.FileSystem.RootDirectory}");

			RootDirectory = _config.FileSystem.RootDirectory;
			if (!Directory.Exists(_config.FileSystem.RootDirectory))
				Directory.CreateDirectory(_config.FileSystem.RootDirectory);

			CreateProcessIdLockFile();


			return Task.FromResult(true);
		}

		public string BuildFilePath(string path)
		{
			return RootDirectory + path;
		}

		public bool SaveFile(string file, object obj)
		{
			try
			{
				File.WriteAllText(BuildFilePath(file), obj.ToJson());
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during save file {file} => {ex} ");
				return false;
			}
		}

		public bool SaveFileText(string file, string text)
		{
			File.WriteAllText(BuildFilePath(file), text);
			return true;
		}

		public object LoadFile(string file, Type type)
		{
			try
			{
				return File.ReadAllText(BuildFilePath(file)).FromJson(type);
			}
			catch
			{
				return null;
			}
		}

		public T LoadFile<T>(string file)
		{
			return (T) LoadFile(file, typeof(T));
		}

		public bool CreateDirectory(string directory)
		{
			if (Directory.Exists(BuildFilePath(directory))) return false;

			Directory.CreateDirectory(BuildFilePath(directory));
			return true;
		}

		public Task<bool> Stop()
		{
			RemoveProcessIdLockFile();
			return Task.FromResult(true);
		}

		private void CreateProcessIdLockFile()
		{
			if (File.Exists(BuildFilePath(_pidFile)))
				_logger.LogWarning($"Another process is running with pid {File.ReadAllText(BuildFilePath(_pidFile))}");
			else
				File.WriteAllText(BuildFilePath(_pidFile), Process.GetCurrentProcess().Id.ToString());
		}

		private void RemoveProcessIdLockFile()
		{
			File.Delete(BuildFilePath(_pidFile));
		}
	}
}