using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Gamepedia.Core.Models;
using Gamepedia.Core.Services;

using Microsoft.Toolkit.Uwp.Notifications;

using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace Gamepedia.Background.Lol
{
#nullable enable
	public sealed class FetchData : IBackgroundTask
	{
		private BackgroundTaskDeferral? backgroundTaskDeferral;

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			backgroundTaskDeferral = taskInstance.GetDeferral();
			StorageFile? file = await ApplicationData.Current.LocalFolder.GetFileAsync("news.json");
			IList<News>? news = await JsonSerializer.DeserializeAsync<IList<News>>(await file.OpenStreamForReadAsync()) ?? new List<News>();
			using var leaguepedia = new Leaguepedia();
			ToastContent? toastContent;
			ToastNotification? toast;

			foreach (News _news in leaguepedia.News)
			{
				if (news.Contains(_news) is false)
				{
					// Construct the visuals of the toast (using Notifications library)
					toastContent = _news.Team.Logo is null
						? new ToastContentBuilder()
							.AddToastActivationInfo(
								"action=left&conversationId=5",
								ToastActivationType.Foreground)
							.AddText(_news.Team.Name, hintMaxLines: 1)
							.AddText(_news.Event)
							.GetToastContent()
						: new ToastContentBuilder()
							.AddToastActivationInfo(
								"action=left&conversationId=5",
								ToastActivationType.Foreground)
							.AddAppLogoOverride(new Uri(_news.Team.Logo))
							.AddText(_news.Team.Name, hintMaxLines: 1)
							.AddText(_news.Event)
							.GetToastContent();

					// And create the toast notification
					toast = new ToastNotification(toastContent.GetXml());

					// And then show it
					ToastNotificationManager.CreateToastNotifier().Show(toast);
					news.Insert(0, _news);
					continue;
				}

				break;
			}

			await FileIO.WriteTextAsync(file, JsonSerializer.Serialize(news));
			backgroundTaskDeferral.Complete();
		}
	}
}