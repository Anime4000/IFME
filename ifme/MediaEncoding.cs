using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ifme
{
    public class MediaEncoding
    {        
        private string AppDir { get { return Environment.CurrentDirectory; } }
        private string FFmpeg { get { return FFmpegDotNet.FFmpeg.Main; } }
        private string MKVExtract { get { return Path.Combine(AppDir, "plugin", "mkvtoolnix", "mkvextract"); } }
        private string MKVMerge { get { return Path.Combine(AppDir, "plugin", "mkvtoolnix", "mkvmerge"); } }
        private string MP4Box { get { return Path.Combine(AppDir, "plugin", "mp4box", "MP4Box"); } }
        private string FFms { get { return Path.Combine(AppDir, "plugin", "ffms", "ffmsindex"); } }

        string Temp { get { return Properties.Settings.Default.TempFolder; } }
        string tempVideo { get { return Path.Combine(Temp, "video"); } }
        string tempAudio { get { return Path.Combine(Temp, "audio"); } }
        string tempSubtitle { get { return Path.Combine(Temp, "subtitle"); } }
        string tempAttachment { get { return Path.Combine(Temp, "attachment"); } }

        public MediaEncoding(Queue media)
        {
            ReadyTemp();

            if (Extract(media) == 0)
                if (Indexing(media) == 0)
                    if (Video(media) == 0)
                        if (Audio(media) == 0)
                            if (Muxing(media) == 0)
                                return;

            Backup(Properties.Settings.Default.SaveFolder);
        }

        public void Backup(string targetFolder)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(Temp, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(Temp, targetFolder));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(Temp, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(Temp, targetFolder), true);
        }

        private void ReadyTemp()
        {
            // Clean content
            DirectoryInfo di = new DirectoryInfo(Temp);

            foreach (var file in di.GetFiles())
                file.Delete();

            foreach (var folder in di.GetDirectories())
                folder.Delete();

            // Create folder
            if (!Directory.Exists(Temp))
                Directory.CreateDirectory(Temp);

            if (!Directory.Exists(tempVideo))
                Directory.CreateDirectory(tempVideo);

            if (!Directory.Exists(tempAudio))
                Directory.CreateDirectory(tempAudio);

            if (!Directory.Exists(tempSubtitle))
                Directory.CreateDirectory(tempSubtitle);

            if (!Directory.Exists(tempAttachment))
                Directory.CreateDirectory(tempAttachment);
        }

        private int Extract(Queue item)
        {
            int _sub = 0;
            foreach (var sub in item.Subtitle)
            {
                if (sub.Id == -1)
                {
                    File.Copy(sub.File, Path.Combine(tempSubtitle, $"subtitle{_sub++:D4}_{sub.Lang}.{sub.Format}"));
                }
                else
                {
                    TaskManager.Run($"\"{FFmpeg}\" -hide_banner -loglevel quiet -i \"{sub.File}\" -map 0:{sub.Id} -y {Path.Combine(tempSubtitle, $"subtitle{_sub++:D4}_{sub.Lang}.{sub.Format}")}");
                }
            }

            TaskManager.Run($"\"{FFmpeg}\" -hide_banner -loglevel quiet -dump_attachment:t \"\" -i \"{item.Properties.FilePath}\" -y", tempAttachment);

            TaskManager.Run($"\"{MKVExtract}\" chapters \"{item.Properties.FilePath}\" > chapters.xml");

            return 0;
        }

        private int Indexing(Queue item)
        {
            if (item.Video.Count > 0)
                if (TaskManager.Run($"\"{FFms}\" -p -f -c \"{item.Video[0].File}\" timecode") >= 1)
                    foreach (var tc in Directory.GetFiles(Temp, "timecode*"))
                        File.Delete(tc);

            return 0;
        }

        private int Video(Queue item)
        {
            return 0;
        }

        private int Audio(Queue item)
        {
            return 0;
        }

        private int Muxing(Queue item)
        {
            return 0;
        }
    }
}
