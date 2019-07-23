using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Attributes.OAuth;
using Neon.HomeControl.Api.Core.Data.OAuth;
using Neon.HomeControl.Api.Core.Data.UserInteraction;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.OAuth;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;

namespace Neon.HomeControl.Components.Components
{
	[Component("spotify_web", "Spotify Web", "1.0", "MUSIC", "Connect to spotify Web Api", typeof(SpotifyWebConfig))]
	[OAuthProvider("spotify")]
	public class SpotifyWebComponent : ISpotifyWebComponent, IOAuthCallback
	{
		private const string TokenAuthUrl = "https://accounts.spotify.com/api/token";
		private const string RedirectUrl = "https://localhost:5001/api/oauth/Authorize/spotify";
		private const string AuthorizeUrl = "https://accounts.spotify.com/it/authorize";

		private readonly IComponentsService _componentsService;
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly HttpClient _httpClient;
		private readonly IIoTService _ioTService;
		private readonly ILogger _logger;
		private readonly ISchedulerService _schedulerService;


		private readonly string[] _spotifyScopes =
		{
			"user-read-playback-state",
			"user-modify-playback-state",
			"playlist-read-private",
			"user-read-private",
			"user-read-email"
		};

		private readonly IUserInteractionService _userInteractionService;
		private SpotifyWebConfig _config;

		private SpotifyWebAPI _spotifyWebApi;

		public SpotifyWebComponent(ILogger<SpotifyWebComponent> logger,
			IComponentsService componentsService,
			IUserInteractionService userInteractionService,
			IEventDatabaseService eventDatabaseService,
			IIoTService ioTService,
			ISchedulerService schedulerService,
			IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_schedulerService = schedulerService;
			_componentsService = componentsService;
			_userInteractionService = userInteractionService;
			_ioTService = ioTService;
			_eventDatabaseService = eventDatabaseService;
			_httpClient = httpClientFactory.CreateClient();
		}

		public async void OnOAuthResult(OAuthResult result)
		{
			if (!string.IsNullOrEmpty(result.Code))
			{
				var res = await _httpClient.PostAsync(TokenAuthUrl,
					HttpClientUtils.BuildFormParams(
						new KeyValuePair<string, string>("grant_type", "authorization_code"),
						new KeyValuePair<string, string>("grant_type", "authorization_code"),
						new KeyValuePair<string, string>("code", result.Code),
						new KeyValuePair<string, string>("client_id", _config.ClientId),
						new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
						new KeyValuePair<string, string>("redirect_uri", RedirectUrl)
					));


				if (res.StatusCode == HttpStatusCode.OK)
				{
					var tokenString = await res.Content.ReadAsStringAsync();
					var tokenInfo = tokenString.FromJson<OAuthTokenResult>();
					_logger.LogInformation(
						$"Spotify authentication OK, token expire {DateTime.Now.AddSeconds(tokenInfo.ExpiresIn).ToShortDateString()}");
					_config.AccessToken = tokenInfo.AccessToken;
					_config.TokenType = tokenInfo.TokenType;
					_config.ExpireOn = DateTime.Now.AddSeconds(tokenInfo.ExpiresIn).ToLocalTime();

					_componentsService.SaveComponentConfig(_config);

					_schedulerService.AddJob(async () =>
					{
						_logger.LogInformation("Refresh token");

						res = await _httpClient.PostAsync(TokenAuthUrl,
							HttpClientUtils.BuildFormParams(
								new KeyValuePair<string, string>("grant_type", "refresh_token"),
								new KeyValuePair<string, string>("refresh_token", tokenInfo.RefreshToken),
								new KeyValuePair<string, string>("client_id", _config.ClientId),
								new KeyValuePair<string, string>("client_secret", _config.ClientSecret)));
						var st = await res.Content.ReadAsStringAsync();
						_logger.LogInformation($"new token {st}");

						InitSpotifyClient();
					}, "SpotifyRefreshToken", (int) TimeSpan.FromMinutes(10).TotalSeconds, false);

					await Start();
				}
			}
		}

		public async Task<bool> Start()
		{
			if (!string.IsNullOrEmpty(_config.ClientId) && string.IsNullOrEmpty(_config.AccessToken))
			{
				BuildUserTokenRequest();
			}
			else if (_config.ExpireOn < DateTime.Now)
			{
				BuildUserTokenRequest();
			}
			else
			{
				InitSpotifyClient();
				_schedulerService.AddPolling(PollingRequest, "Spotify_Device",
					SchedulerServicePollingEnum.SHORT_POLLING);
			}

			return true;
		}


		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = (SpotifyWebConfig) config;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new SpotifyWebConfig();
		}

		private void InitSpotifyClient()
		{
			if (_spotifyWebApi != null)
				lock (_spotifyWebApi)
				{
					_spotifyWebApi?.Dispose();
				}

			_spotifyWebApi = new SpotifyWebAPI {TokenType = _config.TokenType, AccessToken = _config.AccessToken};
		}

		private void BuildUserTokenRequest()
		{
			_userInteractionService.AddUserInteractionData(new UserInteractionData
			{
				Name = "SPOTIFY",
				Fields = new List<UserInteractionField>
				{
					new UserInteractionField().Build()
						.SetFieldName("AUTH_URL")
						.SetFieldValue(GenerateOAuthToken(_spotifyScopes))
						.SetFieldType(UserInteractionFieldTypeEnum.LINK)
						.SetDescription("Click on link for authorize spotify API").SetIsRequired(true)
				}
			}, data => { });
		}

		private async void PollingRequest()
		{
			try
			{
				await GetDevices();
				await GetCurrentPlayback();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during polling requests {ex}");
			}
		}

		private async Task GetCurrentPlayback()
		{
			var currentPlayback = await _spotifyWebApi.GetPlaybackAsync();

			if (!currentPlayback.HasError())
			{
				if (currentPlayback.Item != null)
				{
					var currentPlaybackEd = new SporifyCurrentTrackEd
					{
						EntityName = "SPOTIFY_CURRENT_PLAYING",
						ArtistName = currentPlayback.Item.Artists[0].Name,
						SongName = currentPlayback.Item.Name,
						Uri = currentPlayback.Item.Href,
						IsPlaying = currentPlayback.IsPlaying
					};

					_ioTService.InsertEvent(currentPlaybackEd);
				}
			}
			else
			{
				_logger.LogError($"Error during get Devices: {currentPlayback.Error}");
			}
		}

		private async Task GetDevices()
		{
			var devices = await _spotifyWebApi.GetDevicesAsync();

			if (!devices.HasError())
				devices.Devices.ForEach(device =>
				{
					var devEntity = new SpotifyDeviceEd
					{
						VolumePercent = device.VolumePercent,
						DeviceName = device.Name,
						EntityName = device.Name,
						DeviceType = device.Type,
						IsActive = device.IsActive,
						IsRestricted = device.IsRestricted
					};

					_ioTService.InsertEvent(devEntity);
				});
			else
				_logger.LogError($"Error during get Devices: {devices.Error}");
		}

		private string GenerateOAuthToken(params string[] scopes)
		{
			return
				$"{AuthorizeUrl}?response_type=code&redirect_uri={RedirectUrl}&client_id={_config.ClientId}&scope={string.Join("%20", scopes)}&state=k332yl";
		}
	}
}