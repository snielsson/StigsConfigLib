// Copyright © 2014-2018 Stig Schmidt Nielsson. This file is distributed under the MIT license - see LICENSE.txt or https://opensource.org/licenses/MIT. 

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace StigsConfigLib.Tests {
	public sealed class ConfigurationServiceTests {

		public class MyConfig : ConfigurationService<MyConfig> {
			public string Key00 { get; set; }
			// ReSharper disable once InconsistentNaming
			public string key0 { get; set; }
			public string Key1 { get; set; }
			public List<int> Key2 { get; set; }
			public List<string> Key3 { get; set; }
			public List<string> Key4 { get; set; }
			public string KeyWithHyphen { get; set; }
			public string IniKey1 { get; set; }
			public Section1Class Section1 { get; set; }
			public class Section1Class {
				public string IniKey2 { get; set; }
			}
		}
		[Fact]
		public void ReloadOnFileChangeWorks() {
			var target = MyConfig.Load();
			var configurationChanges = new List<(IConfigurationService Service, string Path)>();
			var taskCompletionSource = new TaskCompletionSource<bool>();
			target.ConfigurationChangedEvent += (s, p) => {
				configurationChanges.Add((s, p));
				taskCompletionSource.SetResult(true);
			};

			//Change file.
			var path = "ConfigurationFiles/MyConfig.Default.json";
			var fileContents = File.ReadAllText(path) + "\n";
			configurationChanges.Count.ShouldBe(0);
			File.WriteAllText(path, fileContents);
			taskCompletionSource.Task.Wait();
			configurationChanges.Count.ShouldBe(1);
			configurationChanges[0].Path.ShouldEndWith($"ConfigurationFiles{Path.DirectorySeparatorChar}MyConfig.Default.json");
		}

		[Fact]
		public void Works() {
			var target = MyConfig.Load();
			target.Key00.ShouldBe("default val00");
			target.key0.ShouldBe("default val0");
			target.Key1.ShouldBe("default val1");
			target.Key2.ShouldBe(new[] {1, 2, 3, 4});
			target.Key3.ShouldBe(new[] {"1", "2", "3", "4"});
			target.Key4[0].ShouldBe("5");
			target.Key4[1].ShouldBe("7");
			target.Key4[1].ShouldBe("7");
			target.KeyWithHyphen.ShouldBeNull("Key with hyphens are not valid C# identifiers.");

			target = MyConfig.Load("Staging");
			target.Key00.ShouldBe("default val00");
			target.key0.ShouldBe("default val0");
			target.Key1.ShouldBe("staging val1");

			target = MyConfig.Load("Production");
			target.Key00.ShouldBe("default val00");
			target.key0.ShouldBe("default val0");
			target.Key1.ShouldBe("production val1");
		}
	}
}