using System;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Api.Core.Data.Services
{
	public class ServiceInfo
	{
		public IService Service { get; set; }
		public Guid ServiceId { get; set; }

		public string Name { get; set; }

		public ServiceStatusEnum Status { get; set; }

		public Exception Exception { get; set; }
	}
}