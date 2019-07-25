using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IFileSystemManager : IService
	{
		string RootDirectory { get; set; }

		string BuildFilePath(string path);

		bool SaveFile(string file, object obj);

		bool SaveFileText(string file, string text);

		object LoadFile(string file, Type type);

		T LoadFile<T>(string file);

		bool CreateDirectory(string directory);
	}
}