using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IFME
{
	public class Plugins
	{
		public class Items
		{
			public static Dictionary<Guid, PluginsCommon> Lists { get; set; } = new Dictionary<Guid, PluginsCommon>();

            public static Dictionary<Guid, PluginsAudio> Audio { get; set; } = new Dictionary<Guid, PluginsAudio>();
			public static Dictionary<Guid, PluginsVideo> Video { get; set; } = new Dictionary<Guid, PluginsVideo>();
		}
	}

	public class PluginsCommon
	{
		public Guid GUID { get; set; }
		public string Name { get; set; }
		public string Version { get; set; }
		public bool X64 { get; set; }
		public bool TestRequired { get; set; } = false;
		public string[] Format { get; set; }
		public PluginsAuthor Author { get; set; } = new PluginsAuthor();
		public PluginsUpdate Update { get; set; } = new PluginsUpdate();
	}

	public class PluginsAuthor
	{
		public string Developer { get; set; }
		public Uri URL { get; set; }
	}

	public class PluginsUpdate
	{
		public Uri VersionURL { get; set; }
		public Uri DownloadURL { get; set; }
	}
}
