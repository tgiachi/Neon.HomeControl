using System;

namespace Neon.HomeControl.Api.Core.Utils
{
	public static class PropertyChangedNotificationInterceptor
	{
		public static void Intercept(object target, Action onPropertyChangedAction, string propertyName)
		{
			onPropertyChangedAction();
		}
	}
}