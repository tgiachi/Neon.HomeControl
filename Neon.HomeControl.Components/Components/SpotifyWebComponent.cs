using Microsoft.Extensions.Logging;
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
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Commands;
using SpotifyAPI.Web.Enums;

namespace Neon.HomeControl.Components.Components
{
    [Component("spotify_web", "Spotify Web", "1.0", "MUSIC", "Connect to spotify Web Api", typeof(SpotifyWebConfig))]
    [OAuthProvider("spotify")]
    public class SpotifyWebComponent : ISpotifyWebComponent, IOAuthCallback
    {
        private const string TokenAuthUrl = "https://accounts.spotify.com/api/token";
        private const string RedirectUrl = "https://localhost:5001/api/oauth/Authorize/spotify";
        private const string AuthorizeUrl = "https://accounts.spotify.com/authorize";

        private readonly IComponentsService _componentsService;
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
            _httpClient = httpClientFactory.CreateClient();
        }

        public async void OnOAuthResult(OAuthResult result)
        {
            if (!string.IsNullOrEmpty(result.Code))
            {
                var res = await _httpClient.PostAsync(TokenAuthUrl,
                    HttpClientUtils.BuildFormParams(
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
                        $"Spotify authentication OK, token expire {DateTime.Now.AddSeconds(tokenInfo.ExpiresIn).ToString()}");
                    _config.AccessToken = tokenInfo.AccessToken;
                    _config.TokenType = tokenInfo.TokenType;
                    _config.ExpireOn = DateTime.Now.AddSeconds(tokenInfo.ExpiresIn).ToLocalTime();
                    _config.RefreshToken = tokenInfo.RefreshToken;

                    _componentsService.SaveComponentConfig(_config);

                    //	RefreshTokenJob();
                    await Start();
                }
            }
        }

        private void RefreshTokenJob()
        {
            _logger.LogInformation($"Adding refresh token job");
            _schedulerService.AddJob(async () => await RefreshToken(), "SpotifyRefreshToken", (int)TimeSpan.FromMinutes(30).TotalSeconds, false);

        }

        public async Task<bool> Start()
        {
            if (!string.IsNullOrEmpty(_config.ClientId) && string.IsNullOrEmpty(_config.RefreshToken))
            {
                BuildUserTokenRequest();
            }
            else if (_config.ExpireOn < DateTime.Now)
            {
                await RefreshToken();
            }
            else
            {
                InitSpotifyClient();
                RefreshTokenJob();
                _schedulerService.AddPolling(PollingRequest, "Spotify_Device",
                    SchedulerServicePollingEnum.NORMAL_POLLING);
            }

            return true;
        }


        public Task<bool> Stop()
        {
            return Task.FromResult(true);
        }

        public Task InitConfiguration(object config)
        {
            _config = (SpotifyWebConfig)config;

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

            _spotifyWebApi = new SpotifyWebAPI { TokenType = _config.TokenType, AccessToken = _config.AccessToken };
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

        private async Task RefreshToken()
        {
            _logger.LogInformation("Refresh token");

            var res = await _httpClient.PostAsync(TokenAuthUrl,
                HttpClientUtils.BuildFormParams(
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", _config.RefreshToken),
                    new KeyValuePair<string, string>("client_id", _config.ClientId),
                    new KeyValuePair<string, string>("client_secret", _config.ClientSecret)));
            var st = await res.Content.ReadAsStringAsync();

            var newToken = st.FromJson<OAuthTokenResult>();
            _config.AccessToken = newToken.AccessToken;
            _config.ExpireOn = DateTime.Now.AddSeconds(newToken.ExpiresIn);
            _componentsService.SaveComponentConfig(_config);

            _logger.LogInformation($"Token refresh expire on: {_config.ExpireOn}");

            InitSpotifyClient();
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
                _logger.LogError($"Error during Get Current Playback: {currentPlayback.Error.Status} - {currentPlayback.Error.Message}");
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
                        DeviceId = device.Id,
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
                _logger.LogError($"Error during Get Devices: {devices.Error.Status} - {devices.Error.Message}");
        }

        private string GenerateOAuthToken(params string[] scopes)
        {
            return
                $"{AuthorizeUrl}?response_type=code&redirect_uri={RedirectUrl}&client_id={_config.ClientId}&scope={string.Join("%20", scopes)}&state=k332yl";
        }

        #region Commands

        // SET COMMANDS
        [IotCommand("SET_VOLUME", typeof(SpotifyDeviceEd), "Set volume on current or specified device.")]
        [IotCommandParam("Volume_in_percentage", true)]
        [IotCommandParam("DeviceId", false)]
        public async void SendSetVolumeCommand(SpotifyDeviceEd entity, string commandName, params object[] args)
        {
            var volume = args[0] as string;
            var device = "";

            if (args.Length > 1)
                device = args[1] as string;

            var errorResponse = await _spotifyWebApi.SetVolumeAsync(int.Parse(volume), device);
            if (errorResponse.HasError())
                _logger.LogError($"Error during Set Volume: {errorResponse.Error.Status} - {errorResponse.Error.Message}");

        }

        [IotCommand("SET_REPEAT", typeof(SpotifyDeviceEd), "Set repeat mode on current or specified device.")]
        [IotCommandParam("State", true)] // track, context, off
        [IotCommandParam("DeviceId", false)]
        public async void SendSetRepeatCommand(SpotifyDeviceEd entity, string commandName, params object[] args)
        {
            var state = args[0] as string;
            var device = "";

            if (args.Length > 1)
                device = args[1] as string;

            var errorResponse = await _spotifyWebApi.SetRepeatModeAsync((RepeatState)Enum.Parse(typeof(RepeatState), state, true), device);
            if (errorResponse.HasError())
                _logger.LogError($"Error during Set Repeat: {errorResponse.Error.Status} - {errorResponse.Error.Message}");

        }

        [IotCommand("SET_SHUFFLE", typeof(SpotifyDeviceEd), "Set shuffle mode on current or specified device.")]
        [IotCommandParam("State", true)] // true, false
        [IotCommandParam("DeviceId", false)]
        public async void SendSetShuffleCommand(SpotifyDeviceEd entity, string commandName, params object[] args)
        {
            var state = args[0] as string;
            var device = "";

            if (args.Length > 1)
                device = args[1] as string;

            var errorResponse = await _spotifyWebApi.SetShuffleAsync(bool.Parse(state), device);
            if (errorResponse.HasError())
                _logger.LogError($"Error during Set Shuffle: {errorResponse.Error.Status} - {errorResponse.Error.Message}");
        }


        // PAUSE COMMAND
        [IotCommand("PAUSE", typeof(SpotifyDeviceEd), "Pauses playing on current device.")]
        public async void SendPauseCommand(SpotifyDeviceEd entity, string commandName, params object[] args)
        {
            var errorResponse = await _spotifyWebApi.PausePlaybackAsync();
            if (errorResponse.HasError())
                _logger.LogError($"Error during Pause: {errorResponse.Error.Status} - {errorResponse.Error.Message}");
        }


        // SKIP COMMANDS
        [IotCommand("NEXT_TRACK", typeof(SpotifyDeviceEd), "Skip and move to the next track on current or specified device.")]
        [IotCommandParam("DeviceId", false)]
        public async void SendNextTrackCommand(SpotifyDeviceEd entity, string commandName, params object[] args)
        {
            var errorResponse = await _spotifyWebApi.SkipPlaybackToNextAsync(deviceId: args.Length > 0 ? args[0] as string : "");
            if (errorResponse.HasError())
                _logger.LogError($"Error during Next Track: {errorResponse.Error.Status} - {errorResponse.Error.Message}");
        }

        [IotCommand("PREVIOUS_TRACK", typeof(SpotifyDeviceEd), "Skip and move to the previous track on current or specified device.")]
        [IotCommandParam("DeviceId", false)]
        public async void SendPreviousTrackCommand(SpotifyDeviceEd entity, string commandName, params object[] args)
        {
            var errorResponse = await _spotifyWebApi.SkipPlaybackToPreviousAsync(deviceId: args.Length > 0 ? args[0] as string : "");
            if (errorResponse.HasError())
                _logger.LogError($"Error during Previous Track: {errorResponse.Error.Status} - {errorResponse.Error.Message}");
        }


        // PLAY COMMANDS
        [IotCommand("PLAY_TRACK", typeof(SpotifyDeviceEd), "Play a track on current or specified device.")]
        [IotCommandParam("TrackUri", true)]
        [IotCommandParam("DeviceId", false)]
        public async void SendPlayTrackCommand(SpotifyDeviceEd entity, string commandName, params object[] args) =>
            await SendPlayCommand(trackUri: args[0] as string, deviceId: args.Length > 1 ? args[1] as string : "");

        [IotCommand("PLAY_CONTEXT", typeof(SpotifyDeviceEd), "Play an artist/album/playlist on current or specified device.")]
        [IotCommandParam("ContextUri", true)]
        [IotCommandParam("DeviceId", false)]
        public async void SendPlayContextCommand(SpotifyDeviceEd entity, string commandName, params object[] args) =>
            await SendPlayCommand(contextUri: args[0] as string, deviceId: args.Length > 1 ? args[1] as string : "");

        [IotCommand("RESUME", typeof(SpotifyDeviceEd), "Resume playing on current or specified device.")]
        [IotCommandParam("DeviceId", false)]
        public async void SendResumeCommand(SpotifyDeviceEd entity, string commandName, params object[] args) =>
            await SendPlayCommand(deviceId: args.Length > 0 ? args[0] as string : "");

        private async Task SendPlayCommand(string trackUri = null, string contextUri = null, string deviceId = "")
        {
            List<string> uris = null;
            if (!string.IsNullOrEmpty(trackUri))
                uris = new List<string> { trackUri };

            var errorResponse = await _spotifyWebApi.ResumePlaybackAsync(uris: uris, deviceId: deviceId, offset: (string)null);
            if (errorResponse.HasError())
                _logger.LogError($"Error during Play: {errorResponse.Error.Status} - {errorResponse.Error.Message}");
        }

        #endregion
    }
}