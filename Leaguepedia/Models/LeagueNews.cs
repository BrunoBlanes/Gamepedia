using System;

namespace Gamepedia.Leaguepedia.Models
{
	public sealed class News
	{
		public string Region { get; set; }
		public string Team { get; set; }
		public string Event { get; set; }
		public DateTimeOffset Date { get; set; }

		public News()
		{
			Team = string.Empty;
			Event = string.Empty;
			Region = string.Empty;
		}
	}
}