using Microsoft.Toolkit.Uwp.Notifications;

using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace Gamepedia.Background.Leaguepedia
{
	public sealed class FetchData : IBackgroundTask
	{
		private BackgroundTaskDeferral backgroundTaskDeferral;

		public void Run(IBackgroundTaskInstance taskInstance)
		{
			backgroundTaskDeferral = taskInstance.GetDeferral();

			// Construct the visuals of the toast (using Notifications library)
			ToastContent toastContent = new ToastContentBuilder()
				.AddToastActivationInfo(
					"action=left&conversationId=5",
					ToastActivationType.Foreground)
				.AddText("Testing notifications!")
				.GetToastContent();

			// And create the toast notification
			var toast = new ToastNotification(toastContent.GetXml());

			// And then show it
			ToastNotificationManager.CreateToastNotifier().Show(toast);
			backgroundTaskDeferral.Complete();
		}
	}
}