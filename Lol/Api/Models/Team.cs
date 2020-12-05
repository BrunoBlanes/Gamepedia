using System;
using System.Collections.Generic;
using System.Text;

namespace Gamepedia.Lol.Api.Models
{
	public class Team
	{
		public string Name { get; set; }
		public string OverviewPage { get; set; }

		/// <summary>
		/// The short name of the team.
		/// </summary>
		/// <remarks>
		/// This should agree with Riot always (except for like Academy teams).
		/// </remarks>
		public string Short { get; set; }
		public string Location { get; set; }
		public string TeamLocation { get; set; }
		public string Region { get; set; }
		public string OrganizationPage { get; set; }
		public string Image { get; set; }
		public string Twitter { get; set; }
		public string YouTube { get; set; }
		public string Facebook { get; set; }
		public string Instagram { get; set; }
		public string Discord { get; set; }
		public string Snapchat { get; set; }
		public string Vk { get; set; }
		public string Subreddit { get; set; }

		/// <summary>
		/// For PMT.
		/// </summary>
		public string Website { get; set; }
		public string RosterPhoto { get; set; }
		public bool IsDisbanded { get; set; }

		/// <summary>
		/// Name of the team renamed to or <c>null</c>.
		/// </summary>
		public string? RenamedTo { get; set; }
		public bool IsLowercase { get; set; }

		public Team()
		{

		}
	}
}