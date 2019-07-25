using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Neon.HomeControl.Service
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var isService = !(Debugger.IsAttached || args.Contains("--console"));

			if (isService)
			{
				var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
				var pathToContentRoot = Path.GetDirectoryName(pathToExe);
				Directory.SetCurrentDirectory(pathToContentRoot);
			}

			var builder = Web.Program.CreateWebHostBuilder(
				args.Where(arg => arg != "--console").ToArray());

			var host = builder.Build();

			if (isService)
				// To run the app without the CustomWebHostService change the
				// next line to host.RunAsService();
				host.RunAsService();
			else
				host.Run();
		}
	}
}