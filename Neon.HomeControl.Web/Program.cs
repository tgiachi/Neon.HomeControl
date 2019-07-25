using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;

namespace Neon.HomeControl.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.Filter.ByExcluding(Matching.FromSource("Microsoft"))
				.Filter
				.ByExcluding(Matching.FromSource("System"))
				.Enrich.FromLogContext()
				.MinimumLevel.Information()
				.WriteTo.File(new CompactJsonFormatter(), "Logs/Neon.homecontrol.log",
					rollingInterval: RollingInterval.Day)
				.WriteTo.Console(
					theme: AnsiConsoleTheme.Literate,
					outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] [{SourceContext:u3}] {Message}{NewLine}{Exception}")
				.CreateLogger();
			try
			{
				CreateWebHostBuilder(args).Build().Run();
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.ConfigureLogging(builder => builder.AddDebug())
				.UseSerilog()
				.UseStartup<Startup>();
		}
	}
}