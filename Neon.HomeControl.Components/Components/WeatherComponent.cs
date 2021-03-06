﻿using DarkSky.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Commands;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Data.Commands;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neon.HomeControl.Components.Components
{
	[Component("weather", "Weather", "1.0", "WEATHER", "Broadcast weather", typeof(WeatherComponentConfig))]
	public class WeatherComponent : IWeatherComponent
	{
		private readonly IIoTService _ioTService;
		private readonly ILogger _logger;
		private readonly ISchedulerService _schedulerService;
		private WeatherComponentConfig _config;
		private DarkSkyService _darkSkyService;

		public WeatherComponent(ISchedulerService jobSchedulerService, ILogger<WeatherComponent> logger, IIoTService ioTService)
		{
			_logger = logger;
			_schedulerService = jobSchedulerService;

			_ioTService = ioTService;
		}

		public Task<bool> Start()
		{
			if (_config.ApiKey != "ChangeMe")
			{
				_darkSkyService = new DarkSkyService(_config.ApiKey);
				_logger.LogInformation("Check Weather every 3 minutes");
				_schedulerService.AddJob(GetWeather, (int)TimeSpan.FromMinutes(3).TotalSeconds, true);
				return Task.FromResult(true);
			}

			throw new Exception("Configuration Needed");
		}

		public Task<bool> Stop()
		{
			_darkSkyService = null;
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = config as WeatherComponentConfig;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new WeatherComponentConfig();
		}

		private async void GetWeather()
		{
			var forecast = await _darkSkyService.GetForecast(_config.Latitude, _config.Longitude,
				new DarkSkyService.OptionalParameters
				{
					ExtendHourly = true,
					DataBlocksToExclude = new List<ExclusionBlock> { ExclusionBlock.Flags },
					LanguageCode = "x-pig-latin",
					MeasurementUnits = "si"
				});

			_logger.LogInformation(
				$"{forecast.Response.Currently.Icon} Temperature {forecast.Response.Currently.Temperature} - Humidity {forecast.Response.Currently.Humidity * 100}%");


			var entity = new WeatherEd
			{
				EntityName = "Weather",
				EventDateTime = DateTime.Now,
				Humidity = forecast.Response.Currently.Humidity.Value,
				Temperature = forecast.Response.Currently.Temperature.Value,
				Pressure = forecast.Response.Currently.Pressure.Value,
				WindBearing = forecast.Response.Currently.WindBearing.Value,
				UvIndex = forecast.Response.Currently.UvIndex.Value,
				WindGust = forecast.Response.Currently.WindGust.Value,
				WindSpeed = forecast.Response.Currently.WindSpeed.Value,
				Status = forecast.Response.Currently.Icon.ToString()
			};

			_ioTService.InsertEvent(entity);
		}

		/// <summary>
		/// Command rise
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="commandName"></param>
		/// <param name="args"></param>
		[IotCommand("RISE", typeof(WeatherEd), "Set sun up in the sky")]
		[IotCommandParam("Azimut", false)]
		public void RiseCommand(WeatherEd entity, string commandName, params object[] args)
		{
			_logger.LogInformation("This is RISE command name");
		}

	}
}