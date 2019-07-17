using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.UserInteraction;
using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IUserInteractionService), LoadAtStartup = true, Name = "User interaction Service")]
	public class UserInteractionService : IUserInteractionService
	{
		private readonly Dictionary<string, Action<UserInteractionData>> _configNotifiers =
			new Dictionary<string, Action<UserInteractionData>>();

		public UserInteractionService()
		{
			UserInteractionData = new ObservableCollection<UserInteractionData>();
		}

		public ObservableCollection<UserInteractionData> UserInteractionData { get; set; }

		public List<UserInteractionData> NeedUserInteractionData => UserInteractionData.ToList();

		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public void AddUserInteractionData(UserInteractionData data, Action<UserInteractionData> onConfigAdd)
		{
			_configNotifiers.Add(data.Name, onConfigAdd);
			UserInteractionData.Add(data);
		}

		public void CompileEntry(string name, string field, object value)
		{
			var ui = UserInteractionData.ToList().FirstOrDefault(s => s.Name == name);

			var uiField = ui.Fields.FirstOrDefault(f => f.FieldName == field);

			uiField.FieldValue = value;

			_configNotifiers[name](ui);
		}
	}
}