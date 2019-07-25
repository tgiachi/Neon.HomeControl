using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using System;

namespace Neon.HomeControl.Api.Core.Modules
{
	public class LogRequestModule : Module
	{
		private readonly ILogger _logger;
		public int depth;


		protected void AttachToComponentRegistration(IComponentRegistry componentRegistry,
			IComponentRegistration registration, ILogger<LogRequestModule> logger)
		{
			registration.Preparing += RegistrationOnPreparing;
			registration.Activating += RegistrationOnActivating;
			base.AttachToComponentRegistration(componentRegistry, registration);
		}

		private string GetPrefix()
		{
			return new string('-', depth * 2);
		}

		private void RegistrationOnPreparing(object sender, PreparingEventArgs preparingEventArgs)
		{
			Console.WriteLine("{0}Resolving  {1}", GetPrefix(), preparingEventArgs.Component.Activator.LimitType);
			depth++;
		}

		private void RegistrationOnActivating(object sender, ActivatingEventArgs<object> activatingEventArgs)
		{
			depth--;
			Console.WriteLine("{0}Activating {1}", GetPrefix(), activatingEventArgs.Component.Activator.LimitType);
		}
	}
}