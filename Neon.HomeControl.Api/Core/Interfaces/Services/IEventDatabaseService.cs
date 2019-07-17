using System;
using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IEventDatabaseService : IService
	{
		T Insert<T>(T value) where T : IIotEntity;

		T Update<T>(T value) where T : IIotEntity;

		List<T> List<T>() where T : IIotEntity;

		List<object> List(Type type);

		Dictionary<string, List<object>> GetAllEvents();

		List<object> GetEventsByName(string collection);

		List<string> GetEventsName();
	}
}