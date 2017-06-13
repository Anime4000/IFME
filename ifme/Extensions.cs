using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
	static class Extensions
	{
		internal static bool IsOneOf<T>(this T value, params T[] items)
		{
			for (int i = 0; i < items.Length; ++i)
			{
				if (items[i].Equals(value))
				{
					return true;
				}
			}

			return false;
		}
	}
}
