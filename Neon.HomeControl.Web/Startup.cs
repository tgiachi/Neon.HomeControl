using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Logger;
using Neon.HomeControl.Api.Core.Managers;
using Neon.HomeControl.Api.Core.Utils;
using Neon.HomeControl.Entities.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Text;

namespace Neon.HomeControl.Web
{
	public class Startup
	{
		private readonly IServicesManager _servicesManager;
		private IContainer _container;
		private NeonConfig _neonConfig;

		public Startup(IHostingEnvironment env, ILogger<ServicesManager> _serviceManagerLogger)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
				.AddJsonFile("neon.settings.json", true, true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			if (CheckIfDockerContainer())
			{
				_neonConfig = File.ReadAllText("/config/neon.settings.json").FromJson<NeonConfig>();
			}
			else
			{
				_neonConfig = Configuration.Get<NeonConfig>();

				if (_neonConfig == null)
				{
					_neonConfig = new NeonConfig();
					File.WriteAllText(env.ContentRootPath + "neon.settings.json", _neonConfig.ToJson());
				}
			}

			_servicesManager = new ServicesManager(_serviceManagerLogger, _neonConfig);
		}


		public IConfigurationRoot Configuration { get; }

		private bool CheckIfDockerContainer()
		{
			return Environment.GetEnvironmentVariables()["DOTNET_RUNNING_IN_CONTAINER"] != null;
		}


		private async void OnShutdown()
		{
			await _servicesManager.Stop();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var builder = _servicesManager.InitContainer();

			services.AddSingleton(typeof(ILogger<>), typeof(LoggerEx<>));
			services.AddAutoMapper(AssemblyUtils.GetAppAssemblies());
			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			//	if (_neonConfig.EnableMetrics)
			//		services.AddMetrics();


			services.AddLogging();
			services.AddHttpClient();


			var key = Encoding.ASCII.GetBytes(Configuration.Get<NeonConfig>().JwtToken);
			services.AddAuthentication(x =>
				{
					x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(x =>
				{
					x.RequireHttpsMetadata = false;
					x.SaveToken = true;
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false
					};
				});

			if (_neonConfig.EnableSwagger)
				services.AddSwaggerGen(c =>
				{
					c.SwaggerDoc("v1", new Info { Title = "Leon Home control", Version = "v1.0" });
				});

			services.AddDbContextPool<NeonDbContext>(options =>
			{
				//var config = Configuration.Get<NeonConfig>();

				var dbFullPath =
					$"Data Source={_neonConfig.FileSystem.RootDirectory}{_neonConfig.EventsDatabase.DatabaseDirectory}{Path.DirectorySeparatorChar}{_neonConfig.Database.ConnectionString}";
				options.UseSqlite(dbFullPath);
			});

			services.AddTransient<DbContext, NeonDbContext>();
			services.AddResponseCompression();
			builder.Populate(services);
			_container = _servicesManager.Build();

			return new AutofacServiceProvider(_container);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public async void Configure(IApplicationBuilder app, IHostingEnvironment env,
			IApplicationLifetime applicationLifetime)
		{
			applicationLifetime.ApplicationStopping.Register(async () =>
				await _servicesManager.Stop());

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();

			app.UseHttpsRedirection();

			app.UseSwagger();

			app.UseCors(x => x
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());
			app.UseAuthentication();

			if (_neonConfig.EnableSwagger)
				app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leon Home control"); });

			app.UseResponseCompression();

			app.UseMvc();
			await _servicesManager.Start();
		}
	}
}