using System.Collections.ObjectModel;

using Gamepedia.Leaguepedia.Models;
using Gamepedia.Leaguepedia.Services;

using Microsoft.UI.Xaml;

namespace Gamepedia.Leaguepedia
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public ObservableCollection<News> News { get; } = new();

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void LeagueNewsListView_Loading(FrameworkElement sender, object args)
		{
			foreach (var news in await Client.GetNewsAsync())
			{
				News.Add(news);
			}
		}
	}
}