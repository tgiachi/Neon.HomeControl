using System.Collections.Generic;

namespace Neon.HomeControl.Components.AirCo.Model.Response
{
	public class DeviceGroupResponse
	{
		public int GroupCount { get; set; }
		public List<Group> GroupList { get; set; }
	}
}