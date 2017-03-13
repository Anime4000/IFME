using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class MediaQueueCommon
    {
        public bool Enable { get; set; }
        public int Id { get; set; }
		public float Duration { get; set; }
        public string Lang { get; set; }
        public string File { get; set; }
        public string Format { get; set; }
    }
}
