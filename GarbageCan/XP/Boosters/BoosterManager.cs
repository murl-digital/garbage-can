using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using GarbageCan.XP.Data;
using GarbageCan.XP.Data.Entities;
using GarbageCan.XP.Data.Models;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.XP.Boosters
{
	public class BoosterManager : IFeature
	{
		private static List<AvailableSlot> _availableSlots;
		private static List<ActiveBooster> _activeBoosters = new EditableList<ActiveBooster>();
		private static Queue<QueuedBooster> _queuedBoosters;
		
		private static Timer _boosterTimer = new Timer(5000);
		public void Init(DiscordClient client)
		{
			using (var context = new XPContext())
			{
				_availableSlots = context.xpAvailableSlots
					.Select(slot => new AvailableSlot {channelId = slot.channel_id, id = slot.id})
					.ToList();
			}
			
			client.Ready += (sender, args) =>
			{
				Task.Run(Ready);
				
				_boosterTimer.Elapsed += (o, eventArgs) => Tick();

				_boosterTimer.Enabled = true;
				
				return Task.CompletedTask;
			};
		}

		public void Cleanup()
		{
			using (var context = new XPContext())
			{
				_boosterTimer.Enabled = false;
				foreach (var booster in _activeBoosters)
				{
					if (context.xpActiveBoosters.Any(x => x.slot.id == booster.slot.id)) continue;
					context.xpActiveBoosters.Add(new EntityActiveBooster
					{
						expiration_date = booster.expirationDate,
						multipler = booster.multipler,
						slot = context.xpAvailableSlots.Find(booster.slot.id)
					});
				}

				context.SaveChanges();
				context.xpQueuedBoosters.Delete();
				int position = 0;
				foreach (var booster in _queuedBoosters)
				{
					context.xpQueuedBoosters.Add(new EntityQueuedBooster
					{
						duration_in_seconds = booster.durationInSeconds,
						multiplier = booster.multiplier,
						position = position
					});
					position++;
				}

				context.SaveChanges();
			}
		}

		private static ActiveBooster ActivateQueuedBooster(out string slotName, AvailableSlot slot)
		{
			Log.Information("test");
			QueuedBooster queuedBooster = _queuedBoosters.Dequeue();

			ActiveBooster booster = new ActiveBooster
			{
				expirationDate = DateTime.Now.ToUniversalTime()
					.AddSeconds(queuedBooster.durationInSeconds),
				multipler = queuedBooster.multiplier,
				slot = slot
			};

			_activeBoosters.Add(booster);

			using (var context = new XPContext())
			{
				context.xpActiveBoosters.Add(new EntityActiveBooster
				{
					expiration_date = booster.expirationDate,
					multipler = booster.multipler,
					slot = context.xpAvailableSlots.Find(booster.slot.id)
				});
				context.SaveChanges();
			}

			slotName = booster.multipler.ToString(CultureInfo.InvariantCulture);
			return booster;
		}

		private static void Ready()
		{
			try
			{
				using var context = new XPContext();
				List<EntityQueuedBooster> queuedEntities = context.xpQueuedBoosters
					.Select(x => x)
					.ToList();
				queuedEntities.Sort((x, y) => x.position.CompareTo(y.position));

				List<QueuedBooster> models = queuedEntities.Select(x => new QueuedBooster
					{durationInSeconds = x.duration_in_seconds, multiplier = x.multiplier}).ToList();
				_queuedBoosters = new Queue<QueuedBooster>(models);

				if (context.xpActiveBoosters.Any())
				{
					DateTime now = DateTime.Now.ToUniversalTime();
					if (context.xpActiveBoosters.Any(x => x.expiration_date < now))
					{
						foreach (var expired in context.xpActiveBoosters.Where(x => x.expiration_date < now).ToList())
						{
							context.xpActiveBoosters.Remove(expired);
						}

						context.SaveChanges();
					}
							
					List<EntityActiveBooster> activeEntities = context.xpActiveBoosters
						.Select(x => x)
						.Where(x => x.expiration_date < now)
						.ToList();

					foreach (var entity in activeEntities)
					{
						_activeBoosters.Add(new ActiveBooster
						{
							expirationDate = entity.expiration_date,
							multipler = entity.multipler,
							slot = _availableSlots
								.Select(x => x)
								.First(x => x.id == entity.slot.id)
						});
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		private static void Tick()
		{
			try
			{
				List<ActiveBooster> toProcess = new List<ActiveBooster>();
				foreach (var activeBooster in _activeBoosters)
				{
					if (activeBooster.expirationDate < DateTime.Now.ToUniversalTime())
					{
						using (var context = new XPContext())
						{
							EntityActiveBooster entity = context.xpActiveBoosters
								.First(x => x.slot.id == activeBooster.slot.id);
							context.xpActiveBoosters.Remove(entity);
							context.SaveChanges();
						}
								
						toProcess.Add(activeBooster);
					}
				}
				toProcess.ForEach(async x =>
				{
					_activeBoosters.Remove(x);
					
					string slotName = "-";

					if (_queuedBoosters.Count > 0)
						ActivateQueuedBooster(out slotName, x.slot);


					DiscordChannel channel =
						await GarbageCan.client.GetChannelAsync(
							Convert.ToUInt64(x.slot.channelId));

					await channel.ModifyAsync(model =>
						model.Name = slotName);
				});
				
				while (_queuedBoosters.Count > 0 && _activeBoosters.Count < _availableSlots.Count)
				{
					List<AvailableSlot> usedSlots = _activeBoosters
						.Select(x => x.slot)
						.ToList();
							
					ActiveBooster booster = ActivateQueuedBooster(out var slotName, _availableSlots
						.First(slot => !usedSlots.Contains(slot)));
							
					Task.Run(async () =>
					{
						DiscordChannel channel =
							await GarbageCan.client.GetChannelAsync(
								Convert.ToUInt64(booster.slot.channelId));

						await channel.ModifyAsync(model =>
							model.Name = slotName);
					});
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public static float GetMultiplier()
		{
			return 1 + _activeBoosters.Sum(booster => booster.multipler - 1);
		} 
	}
}