using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ifme
{
	class Stuff
	{
		/// <summary>
		/// Change and override MKV "Writing application"
		/// </summary>
		/// <param name="file">MKV file</param>
		public static void WriteMkvTagWApp(string file)
		{
			byte[] bytes = { 0x49, 0x46, 0x4D, 0x45, 0x20, 0x34, 0x2E, 0x78, 0x20, 
							   0xE3, 0x80, 0x8C, 0xE3, 0x83, 0x8D, 0xE3, 0x83, 0xA0, 
							   0xE3, 0x81, 0x95, 0xE3, 0x82, 0x93, 0xE3, 0x81, 0xAE, 
							   0xE6, 0x9C, 0xAA, 0xE6, 0x9D, 0xA5, 0xE3, 0x80, 0x8D };

			using (var stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
			{
				stream.Position = 4205;
				for (int i = 0; stream.Position != 4270; i++)
				{
					if (i < bytes.Length)
						stream.WriteByte(bytes[i]);
					else
						stream.WriteByte(0x00);
				}
			}
		}
	}
}
