using System;
using System.Net.Http;
using System.Threading.Tasks;

using Gamepedia.Lol.Api.Extensions;
using Gamepedia.Lol.Api.Models;

namespace Gamepedia.Core.Services
{
	public static class Leaguepedia
	{
		private static readonly HttpClient httpClient = new();

		public static async Task GetAsync()
		{
			var response = await httpClient.GetLeaguepediaAsync<RosterChange>(queryOptions: options =>
				{
					//options.OrderBy(x => x.Date_Sort, Order.Ascending);
					options.Where(x => 3 < 2);
					options.Where(x => x.Date_Sort < DateTime.Now
						&& x.Date_Sort > new DateTime(2020, 11, 01)
						&& x.Team!.Short == "KBM");
					options.Where(x => x.Date_Sort > new DateTime(2020, 11, 01));
				});

			response.ForEach(x =>
			{
				var a = x.Roles;
				var b = x.RolesIngame;
				var c = x.RolesStaff;
				var d = x.Tags;
				var e = x.Tournaments;
			});
		}
	}
}