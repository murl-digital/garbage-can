using System.Threading.Tasks;
using GarbageCan.Data.Entities;
using GarbageCan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data
{
	public class XPManager
	{
		public static async Task<User> RetrieveUser(string id)
		{
			await using var context = new XPContext();
			var entity = await context.xpUsers.SingleOrDefaultAsync(u => u.id.Equals(id));
			if (entity == null) return null;
			return new User
			{
				id = entity.id,
				lvl = entity.lvl,
				xp = entity.xp
			};
		}

		public static void SaveUser(string id, int lvl, double xp)
		{
			using var context = new XPContext();
			var entity = context.xpUsers.Find(id);

			if (entity == null)
			{
				entity = new EntityUser();
				context.xpUsers.Add(entity);
			}

			entity.id = id;
			entity.lvl = lvl;
			entity.xp = xp;

			context.SaveChangesAsync();
		}

		public static void RemoveUser(string id)
		{
			using var context = new XPContext();
			var entity = context.xpUsers.Find(id);
			if (entity == null) return;

			context.xpUsers.Remove(entity);
			context.SaveChangesAsync();
		}
	}
}