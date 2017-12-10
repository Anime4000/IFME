using System;

namespace ifme
{
    class MediaValidator
    {
        /// <summary>
        /// Check if Encoder has desire Bit Depth support
        /// </summary>
        /// <param name="Encoder">Video encoder to check</param>
        /// <param name="Value">Current video bit depth</param>
        /// <returns>If desire bit depth not found, return default 8 bit</returns>
        internal static int IsValidBitDepth(Guid Encoder, int Value)
        {

            if (Plugin.Items.TryGetValue(Encoder, out Plugin temp))
            {
                foreach (var item in temp.Video.Encoder)
                {
                    if (item.BitDepth == Value)
                    {
                        return Value;
                    }
                }
            }

            return 8;
        }

        /// <summary>
        /// Check selected output format is compatible with current selected encoder, true if compatible, false otherwise
        /// </summary>
        /// <param name="OutFormat">Output format to check, example: MP4</param>
        /// <param name="SrcEncoderId">Current selected Encoder GUID</param>
        /// <returns></returns>
        internal static bool IsOutFormatValid(string OutFormat, Guid SrcEncoderId, bool IsVideoType)
        {
            if (Plugin.Items.TryGetValue(SrcEncoderId, out Plugin temp))
            {
                if (Array.Exists(temp.Format, x => x.Equals(OutFormat, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (IsVideoType)
                    {
                        if (temp.Video != null)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (temp.Audio != null)
                        {
                            return true;
                        }
                    }
                    
                }
            }

            return false;
        }

        internal static bool GetCodecVideo(string OutFormat, out MediaQueueVideo Video)
        {
            Video = null;

            foreach (var item in Plugin.Items)
            {
                if (Equals(item.Key, new Guid("00000000-0000-0000-0000-000000000000")))
                    continue;

                if (Array.Exists(item.Value.Format, x => x.Equals(OutFormat, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (!string.IsNullOrEmpty(item.Value.Video.Extension))
                    {
                        Video = new MediaQueueVideo
                        {
                            Encoder = new MediaQueueVideoEncoder
                            {
                                Id = item.Key,
                                Preset = item.Value.Video.PresetDefault,
                                Tune = item.Value.Video.TuneDefault,
                                Mode = 0,
                                Value = item.Value.Video.Mode[0].Value.Default,
                                MultiPass = 2,
                                Command = item.Value.Video.Args.Command
                            }
                        };

                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool GetCodecAudio(string OutFormat, out MediaQueueAudio Audio)
        {
            Audio = null;

            foreach (var item in Plugin.Items)
            {
                if (Equals(item.Key, new Guid("00000000-0000-0000-0000-000000000000")))
                    continue;

                if (Array.Exists(item.Value.Format, x => x.Equals(OutFormat, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (!string.IsNullOrEmpty(item.Value.Audio.Extension))
                    {
                        Audio = new MediaQueueAudio
                        {
                            Encoder = new MediaQueueAudioEncoder
                            {
                                Id = item.Key,
                                Mode = 0,
                                Quality = item.Value.Audio.Mode[0].Default,
                                SampleRate = item.Value.Audio.SampleRateDefault,
                                Channel = item.Value.Audio.ChannelDefault,
                                Command = item.Value.Audio.Args.Command
                            }
                        };

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
