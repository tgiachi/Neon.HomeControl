﻿using System;
using System.Collections.Generic;
using System.Text;
using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class SsdpConfig : BaseComponentConfig
	{
		public bool EnableDiscovery { get; set; }
	}
}