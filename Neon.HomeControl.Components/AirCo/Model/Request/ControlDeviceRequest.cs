using Neon.HomeControl.Components.AirCo.Model.Request;

namespace AircoController.Model.Request
{
	public class ControlDeviceRequest
	{
		public string DeviceGuid { get; set; }
		public ControlDeviceParameters Parameters { get; set; }
	}
}