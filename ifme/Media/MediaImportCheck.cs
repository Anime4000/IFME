using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    class MediaImportCheck
    {
        /// <summary>
        /// Check if Encoder has desire Bit Depth support
        /// </summary>
        /// <param name="Encoder">Video encoder to check</param>
        /// <param name="Value">Current video bit depth</param>
        /// <returns>If desire bit depth not found, return default 8 bit</returns>
        internal static int BitDepth(Guid Encoder, int Value)
        {
            Plugin temp;
            
            if (Plugin.Items.TryGetValue(Encoder, out temp))
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
    }
}
