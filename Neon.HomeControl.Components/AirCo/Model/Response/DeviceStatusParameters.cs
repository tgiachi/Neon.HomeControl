namespace Neon.HomeControl.Components.AirCo.Model.Response
{
	public class DeviceStatusParameters
	{
		public string DevGuid { get; set; }
		public string EventTime { get; set; }
		public OperateType Operate { get; set; }
		public OperationModeType OperationMode { get; set; }
		public decimal TemperatureSet { get; set; }
		public FanSpeedType FanSpeed { get; set; }
		public FanAutoModeType FanAutoMode { get; set; }
		public AirswingLRType AirSwingLR { get; set; }
		public AirSwingUDType AirSwingUD { get; set; }
		public int AirDirection { get; set; }
		public EcoModeType EcoMode { get; set; }
		public int EcoNavi { get; set; }
		public int Nanoe { get; set; }
		public int IAuto { get; set; }
		public int Defrosting { get; set; }
		public string ErrorCode { get; set; }
		public int InsideTemperature { get; set; }
		public int OutTemperature { get; set; }
		public int DevRacCommunicateStatus { get; set; }
		public int ErrorStatus { get; set; }
		public int ActualNanoe { get; set; }
		public int AirQuality { get; set; }
		public long UpdateTime { get; set; }
	}
}