using System;
using Newtonsoft.Json;

namespace Neon.HomeControl.Components.Mqtt
{
	//{"Time":"2019-07-12T08:31:15","Uptime":"0T12:55:26","Vcc":3.182,"SleepMode":"Dynamic","Sleep":50,"LoadAvg":19,"POWER1":"OFF","Wifi":{"AP":1,"SSId":"HASAGIAKI","BSSId":"B0:39:56:F5:6E:AF","Channel":9,"RSSI":62,"LinkCount":4,"Downtime":"0T00:00:34"}}

	public class SonoffStateMessage
	{
		public DateTime Time { get; set; }

		public string Uptime { get; set; }

		public double Vcc { get; set; }

		public string SleepMode { get; set; }

		public int LoadAvg { get; set; }

		[JsonProperty("POWER1")] public string Power1 { get; set; }
	}
}