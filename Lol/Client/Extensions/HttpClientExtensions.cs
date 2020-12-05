using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Gamepedia.Lol.Client.Converters;
using Gamepedia.Lol.Client.Interfaces;
using Gamepedia.Lol.Client.Options;

using Pluralize.NET.Core;

namespace Gamepedia.Lol.Client.Extensions
{
	public static class HttpClientExtensions
	{
		/// <summary>
		/// Runs a query action of type <c>cargodata</c> on the Leaguepedia API.
		/// </summary>
		/// <remarks>
		/// When no parameters are given, this method will return the first <c>50</c> elements of <typeparamref name="T"/> with all their properties.
		/// </remarks>
		/// <typeparam name="T">The Cargo database table or tables on which to search.</typeparam>
		/// <param name="httpClient">The <see cref="HttpClient"/>.</param>
		/// <param name="fields">Columns to return. Defaults to returning all columns.</param>
		/// <param name="limit">A limit on the number of results returned.
		/// No more than <c>500</c> (<c>5000</c> for bots) allowed. Defaults to <c>50</c>.</param>
		/// <param name="offset">Query offset. Defaults to <c>0</c>.</param>
		/// <param name="queryOptions">An action of type <see cref="SqlOptions{T}"/> where aditional SQL queries can be specified, (i.e. <c>WHERE, ORDER BY</c>, etc.).</param>
		/// <param name="format">The format of the output.</param>
		/// <returns>An IList&lt;<typeparamref name="T"/>&gt;.</returns>
		/// <exception cref="InvalidOperationException">When any of <paramref name="fields"/>' child objects are not valid <typeparamref name="T"/> property names.</exception>
		public static async Task<List<T>> GetLeaguepediaAsync<T>(
			this HttpClient httpClient,
			[Optional] List<string>? fields,
			[Optional] ushort? limit,
			[Optional] ushort? offset,
			[Optional] OutputFormat format,
			[Optional] Action<SqlOptions<T>>? queryOptions) where T : class, ICargoTables, new()
		{
			var url = new StringBuilder();
			var where = new StringBuilder();
			var orderBy = new StringBuilder();

			httpClient.BaseAddress = new Uri("https://lol.gamepedia.com");
			url.Append("/api.php?action=cargoquery" +
				$"&format={format.ToString().ToLowerInvariant()}" +
				$"&tables={WebUtility.UrlEncode($"{new Pluralizer().Pluralize(typeof(T).Name)}=A")}" +
				"&utf8=1&formatversion=latest");

			if (limit.HasValue)
			{
				url.Append(limit >= 5000 ? "&limit=max" : $"&limit={limit}");
			}

			if (offset.HasValue)
			{
				url.Append($"&offset={offset}");
			}

			if (queryOptions is not null)
			{
				queryOptions(new SqlOptions<T>(ref fields, ref where, ref orderBy));
			}

			url.Append("&fields=");

			if (fields is null || fields.Count == 0)
			{
				foreach (var propertyInfo in typeof(T).GetProperties())
				{
					url.Append(WebUtility.UrlEncode($"A.{propertyInfo.Name},"));
				}
			}

			else
			{
				foreach (var field in fields)
				{
					if (string.IsNullOrEmpty(field) is false)
					{
						WriteField(field);
					}
				}
			}

			void WriteField(string field)
			{
				foreach (var propertyInfo in typeof(T).GetProperties())
				{
					if (propertyInfo.Name == field)
					{
						url.Append(WebUtility.UrlEncode($"A.{propertyInfo.Name},"));
						return;
					}
				}

				throw new InvalidOperationException($"Field '{field}' is not a valid cell in table '{typeof(T).Name}'.");
			}

			if (string.IsNullOrEmpty(where.ToString()) is false)
			{
				url.Append($"&where={WebUtility.UrlEncode(where.ToString())}");
			}

			if (string.IsNullOrEmpty(orderBy.ToString()) is false)
			{
				url.Append($"&order_by={WebUtility.UrlEncode(orderBy.ToString())}");
			}

			var response = await httpClient.GetAsync(url.ToString());
			response.EnsureSuccessStatusCode();

			if (format == OutputFormat.Json)
			{
				var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
				var jsonValue = "[";

				foreach (var element in document.RootElement.GetProperty("cargoquery").EnumerateArray())
				{
					jsonValue += $"{element.GetProperty("title").GetRawText()},";
				}

				jsonValue = $"{jsonValue.Remove(jsonValue.Length - 1)}]";
				return JsonSerializer.Deserialize<List<T>>(jsonValue, new JsonSerializerOptions
				{
					Converters = { new InternalJsonConverter<T>()},
					Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				}) ?? new List<T>();
			}

			throw new NotImplementedException("Currently only Json output format is supported.");
		}
	}

	public enum OutputFormat
	{
		Json,
		Xml
	}
}