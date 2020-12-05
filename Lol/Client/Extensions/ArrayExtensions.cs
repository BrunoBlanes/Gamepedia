using System;
using System.Collections.Generic;

namespace Gamepedia.Lol.Client.Extensions
{
	public static class ArrayExtensions
	{
		/// <summary>
		/// Creates a <see cref="List{T}"/> from an <see cref="string"/>[].
		/// </summary>
		/// <param name="values">The array of values to convert.</param>
		/// <returns>A <see cref="List{T}"/> that contains all elements from the input
		/// sequence or <c>null</c> if the sequence contains no elements.</returns>
		public static List<T>? ToList<T>(this string[]? values)
		{
			if (values is null || values.Length == 0)
			{
				return null;
			}

			var list = new List<T>();

			foreach (var value in values)
			{
				list.Add((T)Enum.Parse(typeof(T), value));
			}

			return list;
		}
	}
}