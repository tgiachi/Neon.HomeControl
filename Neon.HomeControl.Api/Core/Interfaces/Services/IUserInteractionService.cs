using Neon.HomeControl.Api.Core.Data.UserInteraction;
using System;
using System.Collections.Generic;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IUserInteractionService : IService
	{
		List<UserInteractionData> NeedUserInteractionData { get; }
		void AddUserInteractionData(UserInteractionData data, Action<UserInteractionData> onConfigAdd);

		void CompileEntry(string name, string field, object value);
	}
}