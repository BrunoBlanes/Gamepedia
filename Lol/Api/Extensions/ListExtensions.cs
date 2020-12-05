using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamepedia.Lol.Api.Extensions
{
	public static class ListExtensions
	{
		/// <summary>
		/// Merges all values of <paramref name="list"/> into a <see cref="string"/> using the provided <paramref name="separator"/>.
		/// </summary>
		/// <param name="list">The list of values to merge.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The <see cref="string"/> value or <c>null</c> if <paramref name="list"/> is <c>null</c> or of zero length.</returns>
		public static string ToString(this IList<string>? list, char separator)
		{
			if (list is null || list.Count == 0)
			{
				return string.Empty;
			}

			var result = string.Empty;

			for (int i = 0; i < list.Count; i++)
			{
				result += $"{list[i]}{separator}";
			}

			return result.Remove(result.Length - 1);
		}

		/// <summary>
		/// Merges all values of <paramref name="list"/> into a <see cref="string"/> using the provided <paramref name="separator"/>.
		/// </summary>
		/// <param name="list">The list of values to merge.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The <see cref="string"/> value or <c>null</c> if <paramref name="list"/> is <c>null</c> or of zero length.</returns>
		public static string ToString<T>(this IList<T>? list, char separator) where T : Enum
		{
			if (list is null || list.Count == 0)
			{
				return string.Empty;
			}

			var strings = list.Cast<string>().ToList();
			return ToString(strings, separator);
		}
	}
}