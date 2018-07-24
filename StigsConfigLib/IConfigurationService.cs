// Copyright © 2014-2018 Stig Schmidt Nielsson. This file is distributed under the MIT license - see LICENSE.txt or https://opensource.org/licenses/MIT. 

using System;
using Microsoft.Extensions.Primitives;

namespace StigsConfigLib {
	public interface IConfigurationService { }
	public interface IConfigurationService<out T> : IConfigurationService
	{
		event Action<IConfigurationService<T>, string> ConfigurationChangedEvent;
		void OnConfigurationChangedEvent(string filePath);
	}
}