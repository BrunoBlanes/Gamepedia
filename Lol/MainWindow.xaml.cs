using System;

using Gamepedia.Core.Services;

using Microsoft.UI.Xaml;

namespace Gamepedia.Lol
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window, IDisposable
	{
		private bool disposedValue;

		public Leaguepedia Leaguepedia { get; set; }

		public MainWindow()
		{
			Leaguepedia = new();
			InitializeComponent();
		}

		private void Dispose(bool disposing)
		{
			if (disposedValue is false)
			{
				if (disposing)
				{
					Leaguepedia.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}