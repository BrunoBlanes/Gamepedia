using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Gamepedia.Lol.Client.Extensions;
using Gamepedia.Lol.Client.Interfaces;

namespace Gamepedia.Lol.Client.Models
{
	public class RosterChange : ICargoTables
	{
		private string dateSort;
		/// <summary>
		/// Invisible to the reader, but must be an exact date.
		/// </summary>
		[JsonIgnore]
		public DateTime Date_Sort
		{ 
			get => DateTime.Parse(dateSort);
			set => dateSort = value.ToString("s");
		}

		/// <summary>
		/// The player's name.
		/// </summary>
		public string Player { get; set; } // Turn into a player object

		private string direction;
		/// <summary>
		/// Join or Leave.
		/// </summary>
		[JsonIgnore]
		public Direction Direction
		{
			get => (Direction)Enum.Parse(typeof(Direction), direction);
			set => direction = value.ToString();
		}

		/// <summary>
		/// Team that was joined or left.
		/// </summary>
		public Team? Team { get; set; }

		private string rolesIngame;
		/// <summary>
		/// Only ingame roles.
		/// </summary>
		[JsonIgnore]
		public IList<RolesIngame>? RolesIngame
		{
			get => rolesIngame.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList<RolesIngame>();
			set => rolesIngame = value.ToString(';');
		}

		private string rolesStaff;
		/// <summary>
		/// Only support staff roles.
		/// </summary>
		[JsonIgnore]
		public IList<string>? RolesStaff
		{
			get
			{
				var result = rolesStaff.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				return result.Length > 0 ? result.ToList() : null;
			}

			set => rolesStaff = value.ToString(';');
		}

		private string roles;
		/// <summary>
		/// List of all roles.
		/// </summary>
		[JsonIgnore]
		public IList<string>? Roles
		{
			get
			{
				var result = roles.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				return result.Length > 0 ? result.ToList() : null;
			}

			set => roles = value.ToString(';');
		}

		/// <summary>
		/// Already embeddable.
		/// </summary>
		public string RoleDisplay { get; set; }

		/// <summary>
		/// Temporary legacy support.
		/// </summary>
		[Obsolete("Use one of the newer RoleDisplay or Roles property to get a better and up-to-date information of a player's role.", false)]
		public string? Role { get; set; }

		private string roleModifier;
		/// <summary>
		/// Sub, Trainee or <c>null</c>.
		/// </summary>
		[JsonIgnore]
		public RoleModifier? RoleModifier
		{
			get => string.IsNullOrEmpty(roleModifier) ? null : (RoleModifier)Enum.Parse(typeof(RoleModifier), roleModifier);
			set => roleModifier = value?.ToString() ?? string.Empty;
		}

		/// <summary>
		/// Inactive, etc.
		/// </summary>
		public string Status { get; set; }

		private string currentTeamPriority;
		/// <summary>
		/// Ordering of the team for player pages. <c>1</c> is highest priority.
		/// </summary>
		[JsonIgnore]
		public int CurrentTeamPriority
		{
			get => int.TryParse(currentTeamPriority, out int result) ? result : 0;
			set => currentTeamPriority = value != 0 ? value.ToString() : string.Empty;
		}

		private string playerUnlinked;
		/// <summary>
		/// Should the player's name be unlinked (e.g. support staff distantly removed from esports).
		/// </summary>
		[JsonIgnore]
		public bool PlayerUnlinked
		{
			get => bool.TryParse(playerUnlinked, out bool result) && result;
			set => playerUnlinked = value ? "1" : string.Empty;
		}

		/// <summary>
		/// If preload is <c>set_to_leave_already_joined</c>, this is the name of the team they already joined, otherwise <c>null</c>.
		/// </summary>
		public string? AlreadyJoined { get; set; }

		private string tournaments;
		/// <summary>
		/// For internal use of which tournaments to display the roster changes in.
		/// </summary>
		[JsonIgnore]
		public IList<string>? Tournaments
		{
			get
			{
				var result = tournaments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				return result.Length > 0 ? result.ToList() : null;
			}

			set => tournaments = value.ToString(',');
		}

		/// <summary>
		/// Source for the change.
		/// </summary>
		public string Source { get; set; }

		private string isGCD;
		/// <summary>
		/// Is the change GCD-only? (i.e. no team announce).
		/// </summary>
		[JsonIgnore]
		public bool IsGCD
		{
			get => bool.TryParse(isGCD, out bool result) && result;
			set => isGCD = value? "1" : string.Empty;
		}

		/// <summary>
		/// Mostly for internal use, the preload that the change used so we can filter by type.
		/// </summary>
		public string Preload { get; set; }

		private string preloadSortNumber;
		/// <summary>
		/// For sorting lines within a date.
		/// </summary>
		[JsonIgnore]
		public int PreloadSortNumber
		{
			get => int.TryParse(preloadSortNumber, out int result) ? result : 0;
			set => preloadSortNumber = value != 0 ? value.ToString() : string.Empty;
		}

		private string tags;
		/// <summary>
		/// Users will set yes/no on a set of predefined flags that may add entries to this list. GCD,
		/// </summary>
		[JsonIgnore]
		public IList<string>? Tags
		{
			get
			{
				var result = tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				return result.Length > 0 ? result.ToList() : null;
			}

			set => tags = value.ToString(',');
		}

		/// <summary>
		/// For joining all types of news together.
		/// </summary>
		public string NewsId { get; set; }

		/// <summary>
		/// For joining to Tenures.
		/// </summary>
		public string RosterChangeId { get; set; }

		private string nLineInNews;
		/// <summary>
		/// For sorting roster changes within a single news item (e.g. role change leave then join).
		/// </summary>
		[JsonIgnore]
		public int N_LineInNews
		{
			get => int.TryParse(nLineInNews, out int result) ? result : 0;
			set => nLineInNews = value != 0 ? value.ToString() : string.Empty;
		}

		/// <summary>
		/// Creates a new instance of <see cref="RosterChange"/>.
		/// </summary>
		public RosterChange()
		{
			dateSort = string.Empty;
			Player = string.Empty;
			direction = string.Empty;
			rolesIngame = string.Empty;
			rolesStaff = string.Empty;
			roles = string.Empty;
			RoleDisplay = string.Empty;
			roleModifier = string.Empty;
			Status = string.Empty;
			currentTeamPriority = string.Empty;
			playerUnlinked = string.Empty;
			tournaments = string.Empty;
			Source = string.Empty;
			isGCD = string.Empty;
			Preload = string.Empty;
			preloadSortNumber = string.Empty;
			tags = string.Empty;
			NewsId = string.Empty;
			RosterChangeId = string.Empty;
			nLineInNews = string.Empty;
		}
	}

	public enum Direction
	{
		Join,
		Leave
	}

	public enum RoleModifier
	{
		Sub,
		Trainee
	}

	public enum RolesIngame
	{
		Top,
		Jungle,
		Mid,
		Bot,
		Support
	}
}