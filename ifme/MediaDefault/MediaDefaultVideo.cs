using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
	class MediaDefaultVideo
	{
		public MediaDefaultVideo(MediaTypeVideo type)
		{
			switch (type)
			{
				case MediaTypeVideo.WEBM:
					Encoder = new Guid("deadbeef-9999-9999-9999-999999999999");
					Preset = string.Empty;
					Tune = string.Empty;
					Mode = 0;
					Value = 32;
					Pass = 2;
					Command = string.Empty;
					break;
				case MediaTypeVideo.MP4:
				case MediaTypeVideo.MKV:
				default:
					Encoder = new Guid("deadbeef-0265-0265-0265-026502650265");
					Preset = "medium";
					Tune = "psnr";
					Mode = 0;
					Value = 26;
					Pass = 2;
					Command = "--pme --pmode";
					break;
			}
		}

		public Guid Encoder { get; private set; }
		public string Preset { get; private set; }
		public string Tune { get; private set; }
		public int Mode { get; private set; }
		public int Value { get; private set; }
		public int Pass { get; private set; }
		public string Command { get; private set; }
	}
}
