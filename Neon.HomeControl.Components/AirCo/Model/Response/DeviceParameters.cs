namespace Neon.HomeControl.Components.AirCo.Model.Response
{
	public class DeviceParameters
	{
		public OperateType Operate { get; set; }
		public OperationModeType OperationMode { get; set; }
		public decimal TemperatureSet { get; set; }
		public FanSpeedType FanSpeed { get; set; }
		public FanAutoModeType FanAutoMode { get; set; }
		public AirswingLRType AirSwingLR { get; set; }
		public AirSwingUDType AirSwingUD { get; set; }
		public EcoModeType EcoMode { get; set; }
		public int EcoNavi { get; set; }
		public int Nanoe { get; set; }
		public int IAuto { get; set; }
		public int ActualNanoe { get; set; }
		public int AirDirection { get; set; }
	}
}