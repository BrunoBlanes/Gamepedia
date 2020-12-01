using System;

namespace Gamepedia.Core.Models
{
	public class News : IEquatable<News>
	{
		public string Region { get; set; }
		public Team Team { get; set; }
		public string Event { get; set; }
		public DateTime Date { get; set; }

		public News()
		{
			Team = new();
			Event = string.Empty;
			Region = string.Empty;
		}

		public bool Equals(News news)
		{
			if (news is null)
			{
				return false;
			}

			if (Region == news.Region && Event == news.Event && Team == news.Team)
			{
				return true;
			}

			else
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is null)
			{
				return false;
			}

			if (obj is not News news)
			{
				return false;
			}

			else
			{
				return Equals(news);
			}
		}

		public static bool operator ==(News news1, News new2)
		{
			if (news1 is null || new2 is null)
			{
				return Equals(news1, new2);
			}

			return news1.Equals(new2);
		}

		public static bool operator !=(News news1, News new2)
		{
			return !(news1 == new2);
		}

		public override int GetHashCode()
		{
			return Event.GetHashCode();
		}
	}

	public class Team : IEquatable<Team>
	{
		public string Name { get; set; }
		public string? Logo { get; set; }

		public Team()
		{
			Name = string.Empty;
		}

		public bool Equals(Team team)
		{
			if (team is null)
			{
				return false;
			}

			if (Name == team.Name && Logo == team.Logo)
			{
				return true;
			}

			else
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is null)
			{
				return false;
			}

			if (obj is not Team team)
			{
				return false;
			}

			else
			{
				return Equals(team);
			}
		}

		public static bool operator ==(Team team1, Team team2)
		{
			if (team1 is null || team2 is null)
			{
				return Equals(team1, team2);
			}

			return team1.Equals(team2);
		}

		public static bool operator !=(Team team1, Team team2)
		{
			return !(team1 == team2);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}