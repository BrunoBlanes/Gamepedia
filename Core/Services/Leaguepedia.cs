using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Gamepedia.Core.Models;

using HtmlAgilityPack;

using Windows.Storage;

namespace Gamepedia.Core.Services
{
	public sealed class Leaguepedia : IDisposable
	{
		private const string fileName = "news.json";
		private readonly StorageFolder localFolder;
		private readonly HttpClient httpClient;
		private readonly Uri leaguepedia;
		private readonly Regex regex;
		private bool disposedValue;

		public ObservableCollection<News> News { get; set; }

		public Leaguepedia()
		{
			News = new();
			httpClient = new HttpClient();
			localFolder = ApplicationData.Current.LocalFolder;
			leaguepedia = new Uri($"https://lol.gamepedia.com");
			regex = new(@"(Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?)\s+\d{1,2}");

			Load();
		}

		private async void Load()
		{
			// Ensure the images folder exists
			await localFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

			try
			{
				StorageFile? file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
				using Stream? stream = await file.OpenStreamForWriteAsync();
				IEnumerable<News>? currentNews = await JsonSerializer.DeserializeAsync<IEnumerable<News>>(stream);

				foreach (News? news in currentNews ?? Enumerable.Empty<News>())
				{
					News.Add(news);
				}
			}

			catch (Exception)
			{
				await GetNewsAsync();
				StorageFile? file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
				using Stream? stream = await file.OpenStreamForWriteAsync();
				await JsonSerializer.SerializeAsync(stream, News);
			}
		}

		private async Task<HtmlNode?> FetchDataAsync()
		{
			var requestUri = new Uri(leaguepedia, $"/Leaguepedia:News/{DateTime.Now.Year}/{DateTime.Now:MMMM}?printable=yes");
			HttpResponseMessage response = await httpClient.GetAsync(requestUri);

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

		private async Task GetNewsAsync()
		{
			foreach (HtmlNode? table in (await FetchDataAsync())?.SelectNodes("//table") ?? Enumerable.Empty<HtmlNode>())
			{
				// Finds the news table
				if (table.HasClass("news-table"))
				{
					// Loops through all the news
					foreach (HtmlNode? row in table.FirstChild.ChildNodes)
					{
						News news = new();

						// Gets the news date from the header row
						if (row.FirstChild.OriginalName == "th")
						{
							var dateValue = regex.Match(row.FirstChild.InnerHtml).Value;
							var date = DateTime.ParseExact(dateValue, "MMMM d", CultureInfo.InvariantCulture);
							news.Date = date;
							continue;
						}

						foreach (HtmlNode? cell in row.ChildNodes)
						{
							if (cell.HasClass("news-region"))
							{
								news.Region = cell.InnerText;
							}

							else if (cell.HasClass("news-subject"))
							{
								foreach (HtmlNode? node in cell.FirstChild.ChildNodes)
								{
									if (node.HasClass("teamimage-right"))
									{
										if (node.ChildNodes.FirstOrDefault(x => x.OriginalName == "a") is HtmlNode a)
										{
											var href = a.GetAttributeValue("href", string.Empty);
											var name = a.GetAttributeValue("data-to-id", string.Empty);
											news.Team.Logo = await GetImageAsync(href, name);
										}
									}

									else if (node.HasClass("teamname") || node.HasClass("news-league-text"))
									{
										news.Team.Name = node.InnerText;
									}
								}
							}

							else if (cell.HasClass("news-sentence"))
							{
								news.Event = cell.InnerText;
							}
						}

						News.Add(news);
					}
				}
			}
		}

		private async Task<string?> GetImageAsync(string href, string fileName, int size = 48)
		{
			StorageFolder? images = await localFolder.GetFolderAsync("images");

			if (await images.TryGetItemAsync($"{size}x{size}_{fileName}.png") is StorageFile imageFile)
			{
				return imageFile.Path;
			}

			else
			{
				var requestUri = new Uri(leaguepedia, href);
				HttpResponseMessage response = await httpClient.GetAsync(requestUri);

				if (response.IsSuccessStatusCode)
				{
					HtmlDocument html = new();
					html.Load(await response.Content.ReadAsStreamAsync());

					if (html.DocumentNode is not null)
					{
						HtmlNode? table = html.DocumentNode.SelectNodes("//table").FirstOrDefault(x => x.Id == "infoboxTeam");

						foreach (HtmlNode? row in table?.FirstChild.ChildNodes ?? Enumerable.Empty<HtmlNode>())
						{
							if (row.ChildNodes.FirstOrDefault(x => x.HasClass("infobox-wide")) is HtmlNode cell)
							{
								var src = cell.FirstChild.FirstChild.FirstChild.FirstChild.GetAttributeValue("src", string.Empty);
								href = src.Remove(new Regex(@"\/[0-9]+\?cb=").Match(src).Index);
								response = await httpClient.GetAsync(new Uri($"{href}/{size}"));

								if (response.IsSuccessStatusCode)
								{
									imageFile = await images.CreateFileAsync($"{size}x{size}_{fileName}.png");
									await FileIO.WriteBytesAsync(imageFile, await response.Content.ReadAsByteArrayAsync());
									return imageFile.Path;
								}
							}
						}
					}
				}
			}

			return null;
		}

		private void Dispose(bool disposing)
		{
			if (disposedValue is false)
			{
				if (disposing)
				{
					httpClient.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}