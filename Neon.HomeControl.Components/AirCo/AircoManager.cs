using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AircoController.Model.Request;
using Neon.HomeControl.Components.AirCo.Model;
using Neon.HomeControl.Components.AirCo.Model.Request;
using Neon.HomeControl.Components.AirCo.Model.Response;

namespace Neon.HomeControl.Components.AirCo
{
	public class AircoManager
	{
		private readonly AircoHttpClient _client;

		public AircoManager(HttpClient client)
		{
			_client = new AircoHttpClient(client);
		}

		public void SetAuthorizationToken(string token)
		{
			_client.SetAuthorizationHeader(token);
		}

		public async Task<LoginResponse> Login(string languageId, string loginId, string password)
		{
			var request = new LoginRequest
			{
				Language = languageId,
				LoginId = loginId,
				Password = password
			};
			var response = await _client.PostAsync<LoginResponse, LoginRequest>("auth/login", request);

			_client.SetAuthorizationHeader(response.UToken);

			return response;
		}

		public async Task<DeviceGroupResponse> GetDeviceGroups()
		{
			return await _client.GetAsync<DeviceGroupResponse>("device/group");
		}

		public async Task<DeviceStatusResponse> GetDeviceStatus(string deviceId)
		{
			return await _client.GetAsync<DeviceStatusResponse>($"deviceStatus/now/{HttpUtility.UrlEncode(deviceId)}");
		}

		public async Task<ResultResponse> ControlDevice(string deviceId, OperateType? operate = null,
			OperationModeType? operationMode = null, EcoModeType? ecoMode = null,
			decimal? temperature = null, AirSwingUDType? airSwingUD = null, AirswingLRType? airSwingLR = null,
			FanAutoModeType? fanAutoMode = null, FanSpeedType? fanSpeed = null)
		{
			var request = new ControlDeviceRequest
			{
				DeviceGuid = deviceId,
				Parameters = new ControlDeviceParameters
				{
					Operate = operate,
					OperationMode = operationMode,
					EcoMode = ecoMode,
					TemperatureSet = temperature,
					AirSwingUD = airSwingUD,
					AirSwingLR = airSwingLR,
					FanAutoMode = fanAutoMode,
					FanSpeed = fanSpeed
				}
			};

			return await _client.PostAsync<ResultResponse, ControlDeviceRequest>("deviceStatus/control", request);
		}
	}
}