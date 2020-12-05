using System;

using Gamepedia.Core.Services;

using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;

namespace Gamepedia.Lol
{
#nullable enable
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application
	{
		private Window? window;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeComponent();
			Suspending += OnSuspending;

			// Listen to toast notification activations
			ToastNotificationManagerCompat.OnActivated += e =>
			{
				switch (e.Argument)
				{
					case "left":
						throw new NotImplementedException();
				}
			};
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			await Leaguepedia.GetAsync();
			window = new MainWindow();
			window.Activate();
			var taskRegistered = false;
			await BackgroundExecutionManager.RequestAccessAsync();

			foreach (var task in BackgroundTaskRegistration.AllTasks)
			{
				if (task.Value.Name == "LoadLeaguepediaData")
				{
					taskRegistered = true;
					break;
				}
			}

			if (taskRegistered is false)
			{
				var builder = new BackgroundTaskBuilder
				{
					Name = "LoadLeaguepediaData",
					TaskEntryPoint = "Gamepedia.Background.Lol.FetchData"
				};
				builder.SetTrigger(new TimeTrigger(15, false));
				builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
				builder.Register();
			}
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			// Save application state and stop any background activity
		}
	}
}