using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

using Gamepedia.Core.Models;
using Gamepedia.Core.Services;

using Microsoft.Toolkit.Uwp.Notifications;

using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;
using System.Linq;

namespace Gamepedia.Background.Leaguepedia
{
#nullable enable
	public sealed class FetchData : IBackgroundTask
	{
		private BackgroundTaskDeferral? backgroundTaskDeferral;

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			backgroundTaskDeferral = taskInstance.GetDeferral();
			var file = await ApplicationData.Current.LocalFolder.GetFileAsync("news.json");
			var currentNews = await JsonSerializer.DeserializeAsync<IEnumerable<News>>(await file.OpenStreamForReadAsync());
			var news = await Client.GetNewsAsync();
			ToastContent? toastContent;
			ToastNotification? toast;

			if (currentNews.Count() < news.Count())
			{
				foreach (var newNews in news)
				{
					if (currentNews.Contains(newNews))
					{
						break;
					}
				}

				await JsonSerializer.SerializeAsync(await file.OpenStreamForWriteAsync(), currentNews);

				// Construct the visuals of the toast (using Notifications library)
				toastContent = new ToastContentBuilder()
					.AddToastActivationInfo(
						"action=left&conversationId=5",
						ToastActivationType.Foreground)
					.AddText(currentNews.First().Event)
					.GetToastContent();

				// And create the toast notification
				toast = new ToastNotification(toastContent.GetXml());

				// And then show it
				ToastNotificationManager.CreateToastNotifier().Show(toast);
			}

			// Construct the visuals of the toast (using Notifications library)
			toastContent = new ToastContentBuilder()
				.AddToastActivationInfo(
					"action=left&conversationId=5",
					ToastActivationType.Foreground)
				.AddText("I ran!")
				.GetToastContent();

			// And create the toast notification
			toast = new ToastNotification(toastContent.GetXml());

			// And then show it
			ToastNotificationManager.CreateToastNotifier().Show(toast);
			backgroundTaskDeferral.Complete();
		}
	}
}