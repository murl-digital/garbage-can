﻿using System.Threading.Tasks;
using GarbageCan.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data
{
    public class XPManager
    {
        public static async Task<EntityUser> RetrieveUser(string id)
        {
            await using var context = new XPContext();
            return await context.xp_users.SingleOrDefaultAsync(u => u.id.Equals(id));
        }

        public static void SaveUser(string id, int lvl, double xp)
        {
            using var context = new XPContext();
            var entity = context.xp_users.Find(id);

            if (entity == null)
            {
                entity = new EntityUser();
                context.xp_users.Add(entity);
            }

            entity.id = id;
            entity.lvl = lvl;
            entity.xp = xp;

            context.SaveChangesAsync();
        }

        public static void RemoveUser(string id)
        {
            using var context = new XPContext();
            var entity = context.xp_users.Find(id);
            if (entity == null) return;
            
            context.xp_users.Remove(entity);
            context.SaveChangesAsync();
        }
    }
}