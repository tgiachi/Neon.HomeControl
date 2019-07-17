using System;
using System.IO;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Logger;
using Neon.HomeControl.Api.Core.Managers;
using Neon.HomeControl.Api.Core.Utils;
using Neon.HomeControl.Entities.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Neon.HomeControl.Web
{
	public class Startup
	{
		private readonly IServicesManager _servicesManager;
		private IContainer _container;

		public Startup(IHostingEnvironment env, ILogger<ServicesManager> _serviceManagerLogger)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
				.AddJsonFile("neon.settings.json", true, true)
				.AddEnvironmentVariables();


			Configuration = builder.Build();


			_servicesManager = new ServicesManager(_serviceManagerLogger, Configuration.Get<NeonConfig>());
		}


		public IConfigurationRoot Configuration { get; }


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
				.AddMetrics()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


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

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info {Title = "Leon Home control", Version = "v1.0"});
			});

			//_servicesManager.AddConfiguration(Configuration.Get<NeonConfig>());

			services.AddDbContextPool<NeonDbContext>(options =>
			{
				var config = Configuration.Get<NeonConfig>();

				var dbFullPath =
					$"Data Source={config.FileSystem.RootDirectory}{config.EventsDatabase.DatabaseDirectory}{Path.DirectorySeparatorChar}{config.Database.ConnectionString}";
				options.UseSqlite(dbFullPath);
			});

			services.AddTransient<DbContext, NeonDbContext>();
			//		builder.RegisterType<DbContext>().AsSelf().InstancePerDependency();
			//		builder.RegisterType<NeonDbContext>().As<DbContext>().InstancePerDependency();


			builder.Populate(services);
			_container = _servicesManager.Build();

			return new AutofacServiceProvider(_container);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public async void Configure(IApplicationBuilder app, IHostingEnvironment env,
			IApplicationLifetime applicationLifetime)
		{
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
			app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leon Home control"); });
			app.UseMvc();
			await _servicesManager.Start();
		}
	}
}