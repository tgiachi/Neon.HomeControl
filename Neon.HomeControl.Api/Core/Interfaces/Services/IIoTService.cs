using System;
using System.Linq;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	/// <summary>
	///     Service for save Entities status
	/// </summary>
	public interface IIoTService : IService
	{
		/// <summary>
		///     Insert new entity in IoT database
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		T InsertEntity<T>(T value) where T : IIotEntity;

		/// <summary>
		///     Update Entity
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		T Update<T>(T value) where T : IIotEntity;

		/// <summary>
		///     Get Queryable object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IQueryable<T> Query<T>() where T : IIotEntity;

		/// <summary>
		///     Select entity from ID
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		T FindById<T>(Guid id) where T : IIotEntity;

		/// <summary>
		///     Check if entity exists and add or update
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		T InsertOrUpdate<T>(T value) where T : IIotEntity;

		/// <summary>
		///     Subscribing to this event it is possible to receive entity modifications
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IObservable<T> GetEventStream<T>() where T : IIotEntity;

		/// <summary>
		///  Insert event and save in entities database
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		T InsertEvent<T>(T value) where T : IIotEntity;

		/// <summary>
		///     Publish event modification
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="event"></param>
		void Publish<T>(T @event) where T : IIotEntity;
	}
}