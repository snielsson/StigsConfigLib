// Copyright © 2014-2018 Stig Schmidt Nielsson. This file is distributed under the MIT license - see LICENSE.txt or https://opensource.org/licenses/MIT. 

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using StigsUtilsLib;

namespace StigsConfigLib {
	public static class ConfigurationBuilderExtensions {
		private static bool TryAddFile(this IConfigurationBuilder @this, string path, bool optional = false, bool reloadonChange = false) {
			if (!File.Exists(path)) return false;
			switch (Path.GetExtension(path)) {
				case ".json":
					@this.AddJsonFile(path, optional, reloadonChange);
					return true;
				case ".ini":
					@this.AddIniFile(path, optional, reloadonChange);
					return true;
				case ".xml":
					@this.AddXmlFile(path, optional, reloadonChange);
					return true;
				default: throw new ArgumentException($"Unsupported configuration provider '{path}'.", path);
			}
		}
		public static IConfigurationBuilder Add(this IConfigurationBuilder @this, string path, bool optional = false, bool reloadonChange = false) {
			path = Path.GetFullPath(Path.ChangeExtension(path, ""));
			var xml = @this.TryAddFile(path + ".xml", optional, reloadonChange);
			var json = @this.TryAddFile(path + ".json", optional, reloadonChange);
			var ini = @this.TryAddFile(path + ".ini", optional, reloadonChange);
			Assert.Any($"No xml, json or ini file added for {path}", xml, json, ini);

			return @this;
		}
		public static T Load<T>(this IConfigurationBuilder @this, string env = null, string[] args = null, string name = null, string directory = "ConfigurationFiles", bool optional = true, bool reloadonChange = true) where T : IConfigurationService<T> {
			env = env ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			args = args ?? Environment.GetCommandLineArgs();
			name = name ?? typeof(T).Name;
			@this.Add($"{directory}/{name}.Default.", optional, reloadonChange);
			if (env != null) {
				@this.Add($"{directory}/{name}.{env}.", optional, reloadonChange);
			}
			@this.AddEnvironmentVariables().AddCommandLine(args);
			var configurationRoot = @this.Build();
			var result = configurationRoot.Get<T>();
			if (reloadonChange) {
				foreach (var provider in configurationRoot.Providers.OfType<FileConfigurationProvider>().Where(x => x.Source.ReloadOnChange)) {
					var path = Path.Combine((provider.Source.FileProvider as PhysicalFileProvider)?.Root??"", provider.Source.Path);
					provider.GetReloadToken().RegisterChangeCallback(x => result.OnConfigurationChangedEvent(path), path);
				}
			}
			return result;
		}
	}
}