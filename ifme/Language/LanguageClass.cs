using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ifme
{
	class LanguageClass
	{
		public Font UIFont { get; set; }
		public Dictionary<string, string> frmMain { get; set; } = new Dictionary<string, string>();
		public Dictionary<string, string> frmOption { get; set; } = new Dictionary<string, string>();
		public Dictionary<string, string> frmShutdown { get; set; } = new Dictionary<string, string>();
	}
}
