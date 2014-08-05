using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme.hitoha
{
	class MkvExtractId
	{
		public static string[,] SubtitleData = new string[128, 3]; //ISO, File, ID
		public static string[,] AttachmentData = new string[128, 3]; //File, MIME, ID

		public static string[] Attachment(string input)
		{
			string[] data = new string[3];
			string x = input;

			if (x.Contains("Attachment"))
			{
				string id = null;
				string fn = null;
				string me = null;
				char[] f = x.ToCharArray();

				// Get ID
				for (int i = 0; i < f.Length; i++)
				{
					if (f[i] == ':')
						break;

					if (Char.IsNumber(f[i]))
						id += f[i];
				}

				// Get File Name
				for (int i = f.Length - 1; i > 0; i--)
				{
					if (f[i] == '\'' && f[i - 1] == ' ')
						break;

					if (f[i] == '\'')
						continue;

					fn += f[i];
				}

				char[] tmp = fn.ToCharArray();
				Array.Reverse(tmp);
				fn = new string(tmp);

				// Get Mime
				for (int i = 0; i < f.Length; i++)
				{
					if (f[i] == '\'')
					{
						for (int c = i + 1; c < f.Length; c++)
						{
							if (f[c] == '\'')
								break;

							me += f[c];
						}
						break;
					}
				}

				data[0] = fn;
				data[1] = me;
				data[2] = id;
			}
			return data;
		}
	}
}
