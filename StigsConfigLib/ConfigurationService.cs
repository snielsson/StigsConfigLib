// Copyright © 2014-2018 Stig Schmidt Nielsson. This file is distributed under the MIT license - see LICENSE.txt or https://opensource.org/licenses/MIT. 

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace StigsConfigLib {
	public static class ConfigurationService {
		public static T Load<T>(string env = null, string[] args = null, string name = null, string directory = "ConfigurationFiles", bool optional = true, bool reloadonChange = true) where T : IConfigurationService<T> => new ConfigurationBuilder().Load<T>(env, args, name, directory, optional, reloadonChange);
	}
	public abstract class ConfigurationService<T> : IConfigurationService<T> where T : IConfigurationService<T> {
		public event Action<IConfigurationService<T>, string> ConfigurationChangedEvent;
		public virtual void OnConfigurationChangedEvent(string filePath) {
			ConfigurationChangedEvent?.Invoke(this, filePath);
		}
		public static T Load(string env = null, string[] args = null, string name = null, string directory = "ConfigurationFiles", bool optional = true, bool reloadonChange = true) => ConfigurationService.Load<T>(env,args,name,directory,optional, reloadonChange);
	}
}