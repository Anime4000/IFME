using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class Queue 
    {
        public bool Enable = true;
        public bool MkvOut = true;
        public List<QueueVideo> Video = new List<QueueVideo>();
        public List<QueueAudio> Audio = new List<QueueAudio>();
        public List<QueueSubtitle> Subtitle = new List<QueueSubtitle>();
        public List<QueueAttachment> Attachment = new List<QueueAttachment>();
    }
}
