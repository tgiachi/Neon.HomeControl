using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Data.Plugins
{
	public class PluginConfig
	{
		public string Name { get; set; }

		public string Version { get; set; }

		public string Author { get; set; }
		public string Description { get; set; }

		public List<PluginDependencyConfig> Dependencies { get; set; }

		public PluginConfig()
		{
			Dependencies = new List<PluginDependencyConfig>();
		}

	}
}
