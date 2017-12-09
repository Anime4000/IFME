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
        /// <param name="SrcEncoder">Current selected Encoder GUID</param>
        /// <returns></returns>
        internal static bool IsOutFormatVideoValid(string OutFormat, Guid SrcEncoder)
        {
            if (Plugin.Items.TryGetValue(SrcEncoder, out Plugin temp))
            {
                if (Array.Exists(temp.Format, x => x.Equals(OutFormat, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
