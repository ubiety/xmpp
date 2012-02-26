using System;
using System.Collections.Generic;

namespace ubiety.common.extensions
{
	public static class EnumerableExtensions
	{
		public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> function)
		{
			foreach (var item in enumerable)
			{
				function.Invoke(item);
			}
		}
	}
}
