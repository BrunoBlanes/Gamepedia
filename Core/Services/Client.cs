using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Gamepedia.Core.Models;

using HtmlAgilityPack;

namespace Gamepedia.Core.Services
{
#nullable enable
	public static class Client
	{
		private static readonly Regex regex = new(@"(Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{1,2}");

		public static async Task<HtmlNode?> GetLeaguepediaDataAsync()
		{
			var url = $"https://lol.gamepedia.com/Leaguepedia:News/{DateTime.Now.Year}/{DateTime.Now:MMMM}?printable=yes";
			using var httpClient = new HttpClient();
			HttpResponseMessage response = await httpClient.GetAsync(url);

			if (response.IsSuccessStatusCode)
			{
				HtmlDocument html = new();
				html.Load(await response.Content.ReadAsStreamAsync());

				if (html.DocumentNode is not null)
				{
					return html.DocumentNode;
				}
			}

			return null;
		}

		public static async Task<IEnumerable<News>> GetNewsAsync()
		{
			foreach (var table in (await GetLeaguepediaDataAsync())?.SelectNodes("//table") ?? Enumerable.Empty<HtmlNode>())
			{
				if (table.GetClasses().Contains("news-table"))
				{
					List<News> leagueNews = new();

					foreach (var row in table.FirstChild.ChildNodes)
					{
						News news = new();

						// Gets the news date from the header row
						if (row.FirstChild.OriginalName == "th")
						{
							var dateValue = regex.Match(row.FirstChild.InnerHtml).Value;
							var date = DateTime.ParseExact(dateValue, "MMMM d", CultureInfo.InvariantCulture);
							news.Date = date;
						}

						else
						{
							foreach (var data in row.ChildNodes)
							{
								if (data.GetClasses().Contains("news-region"))
								{
									news.Region = data.InnerText;
								}

								else if (data.GetClasses().Contains("news-subject"))
								{
									news.Team = data.FirstChild.ChildNodes.FirstOrDefault(x =>
										x.GetClasses().Contains("news-league-text")) is HtmlNode node
										? node.InnerText
										: data.FirstChild.ChildNodes.First(x =>
											x.GetClasses().Contains("teamname")).InnerText;
								}

								else if (data.GetClasses().Contains("news-sentence"))
								{
									news.Event = data.InnerText;
								}
							}

							leagueNews.Add(news);
						}
					}

					return leagueNews;
				}
			}

			throw new KeyNotFoundException(@"Could not locate a table with the class ""news-table""");
		}
	}
}