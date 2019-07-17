using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Messages
{
	public interface IBaseMessage
	{
		Guid MessageId { get; set; }
	}
}