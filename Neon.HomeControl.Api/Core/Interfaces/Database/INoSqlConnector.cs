using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;

namespace Neon.HomeControl.Api.Core.Interfaces.Database
{
	/// <summary>
	/// Interface for No SQL database
	/// </summary>
	public interface INoSqlConnector: IDisposable
	{
		Task<bool> Init(string connectionString);

		T Insert<T>(T value, string collectionName) where T : IIotEntity;

		T Update<T>(T value, string collectionName) where T : IIotEntity;

		List<T> List<T>(string collectionName) where T : IIotEntity;

		bool CollectionExists(string name);

		bool AddIndex(string collectionName, string field, bool unique);

		IQueryable<T> Query<T>(string collectionName) where T : IIotEntity;

	}
}
