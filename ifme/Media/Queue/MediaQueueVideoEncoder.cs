using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class MediaQueueVideoEncoder
    {
        public Guid Id { get; set; }
        public string Preset { get; set; }
        public string Tune { get; set; }
        public int Mode { get; set; }
        public decimal Value { get; set; }
        public int MultiPass { get; set; }
        public string Command { get; set; }
    }
}
