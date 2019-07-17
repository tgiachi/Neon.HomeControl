namespace Neon.HomeControl.Components.AirCo.Model.Request
{
	public class ControlDeviceParameters
	{
		public OperateType? Operate { get; set; }
		public OperationModeType? OperationMode { get; set; }
		public EcoModeType? EcoMode { get; set; }
		public int? EcoNavi { get; set; }
		public int? IAuto { get; set; }
		public decimal? TemperatureSet { get; set; }
		public AirSwingUDType? AirSwingUD { get; set; }
		public AirswingLRType? AirSwingLR { get; set; }
		public FanAutoModeType? FanAutoMode { get; set; }
		public FanSpeedType? FanSpeed { get; set; }
	}
}