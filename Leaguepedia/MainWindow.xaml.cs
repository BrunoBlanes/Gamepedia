using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

using Gamepedia.Core.Models;
using Gamepedia.Core.Services;

using Microsoft.UI.Xaml;

using Windows.Storage;

namespace Gamepedia.Leaguepedia
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		private const string fileName = "news.json";

		public ObservableCollection<News> News { get; } = new();

		public MainWindow()
		{
			LoadLeaguepediaData();
			InitializeComponent();
		}

		private async void LoadLeaguepediaData()
		{
			try
			{
				var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
				using var stream = await file.OpenStreamForWriteAsync();
				var currentNews = await JsonSerializer.DeserializeAsync<IEnumerable<News>>(stream);

				foreach (var news in currentNews ?? Enumerable.Empty<News>())
				{
					News.Add(news);
				}
			}

			catch (Exception)
			{
				foreach (var news in await Client.GetNewsAsync())
				{
					News.Add(news);
				}

				var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
				using var stream = await file.OpenStreamForWriteAsync();
				await JsonSerializer.SerializeAsync(stream, News);
			}
		}
	}
}