using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
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
		private static readonly List<ActiveBooster> ActiveBoosters = new List<ActiveBooster>();
		private static Queue<QueuedBooster> _queuedBoosters;

		private static readonly Timer BoosterTimer = new Timer(5000);

		public void Init(DiscordClient client)
		{
			using (var context = new XpContext())
			{
				_availableSlots = context.xpAvailableSlots
					.Select(slot => new AvailableSlot {channelId = slot.channel_id, id = slot.id})
					.ToList();
			}

			client.Ready += (sender, args) =>
			{
				Task.Run(Ready);

				BoosterTimer.Elapsed += Tick;

				BoosterTimer.Enabled = true;

				return Task.CompletedTask;
			};

			client.GuildUpdated += (sender, args) =>
			{
				Log.Information(args.GuildAfter.PremiumSubscriptionCount.ToString());

				if (args.GuildAfter.PremiumSubscriptionCount > args.GuildBefore.PremiumSubscriptionCount)
				{
					AddBooster(2.0f, new TimeSpan(0, 0, 90, 0), true);
				}
				
				return Task.CompletedTask;
			};
		}

		public void Cleanup()
		{
			using var context = new XpContext();
			BoosterTimer.Enabled = false;
			foreach (var booster in ActiveBoosters.Where(booster =>
				!context.xpActiveBoosters.Any(x => x.slot.id == booster.slot.id)))
				context.xpActiveBoosters.Add(new EntityActiveBooster
				{
					expiration_date = booster.expirationDate,
					multipler = booster.multiplier,
					slot = context.xpAvailableSlots.Find(booster.slot.id)
				});

			context.SaveChanges();
			SaveQueue();
		}

		public static float GetMultiplier() => 1 + ActiveBoosters.Sum(booster => booster.multiplier - 1);

		public static void AddBooster(float multiplier, TimeSpan duration, bool queue)
		{
			if (ActiveBoosters.Count >= _availableSlots.Count)
			{
				if (!queue) return;
				
				_queuedBoosters.Enqueue(new QueuedBooster
				{
					multiplier = multiplier,
					durationInSeconds = (long) duration.TotalSeconds
				});
				SaveQueue();
				
			}

			var usedSlots = ActiveBoosters
				.Select(b => b.slot)
				.ToList();

			var slot = _availableSlots
				.First(s => !usedSlots.Contains(s));
			
			var booster = new ActiveBooster
			{
				expirationDate = DateTime.Now.ToUniversalTime().Add(duration),
				multiplier = multiplier,
				slot = slot
			};
			
			ActiveBoosters.Add(booster);

			Task.Run(async () =>
			{
				await using var context = new XpContext();
				await context.xpActiveBoosters.AddAsync(new EntityActiveBooster
				{
					expiration_date = booster.expirationDate,
					multipler = multiplier,
					slot = context.xpAvailableSlots.Find(slot.id)
				});
				await context.SaveChangesAsync();
			});
			
			Task.Run(async () =>
			{
				var channel =
					await GarbageCan.Client.GetChannelAsync(booster.slot.channelId);

				await channel.ModifyAsync(model =>
					model.Name = GetBoosterString(booster));
			});
		}

		private static void Ready()
		{
			try
			{
				using var context = new XpContext();

				#region retrieve queued boosters from db

				var queuedEntities = context.xpQueuedBoosters
					.Select(x => x)
					.ToList();
				queuedEntities.Sort((x, y) => x.position.CompareTo(y.position));

				_queuedBoosters = new Queue<QueuedBooster>(queuedEntities.Select(x => new QueuedBooster
					{durationInSeconds = x.duration_in_seconds, multiplier = x.multiplier}).ToList());

				#endregion

				#region remove any active boosters in db are expired

				var now = DateTime.Now.ToUniversalTime();
				if (context.xpActiveBoosters.Any(x => x.expiration_date < now))
				{
					foreach (var expired in context.xpActiveBoosters.Where(x => x.expiration_date < now).ToList())
						context.xpActiveBoosters.Remove(expired);

					context.SaveChanges();
				}

				#endregion

				#region retrieve remaining active boosters

				foreach (var entity in context.xpActiveBoosters.Where(x => x.expiration_date < now).ToList())
					ActiveBoosters.Add(new ActiveBooster
					{
						expirationDate = entity.expiration_date,
						multiplier = entity.multipler,
						slot = _availableSlots.Select(x => x).First(x => x.id == entity.slot.id)
					});

				#endregion
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			try
			{
				var toProcess = new List<ActiveBooster>();

				using (var context = new XpContext())
				{
					foreach (var activeBooster in ActiveBoosters.Where(activeBooster =>
						activeBooster.expirationDate < DateTime.Now.ToUniversalTime()))
					{
						var entity = context.xpActiveBoosters
							.First(x => x.slot.id == activeBooster.slot.id);
						context.xpActiveBoosters.Remove(entity);

						toProcess.Add(activeBooster);
					}
					
					context.SaveChanges();
				}

				toProcess.ForEach(async x =>
				{
					ActiveBoosters.Remove(x);

					var slotName = "-";

					if (_queuedBoosters.Count > 0)
						ActivateQueuedBooster(out slotName, x.slot);


					var channel =
						await GarbageCan.Client.GetChannelAsync(
							Convert.ToUInt64(x.slot.channelId));

					await channel.ModifyAsync(model =>
						model.Name = slotName);
				});

				var saveQueue = false;

				while (_queuedBoosters.Count > 0 && ActiveBoosters.Count < _availableSlots.Count)
				{
					saveQueue = true;

					var usedSlots = ActiveBoosters
						.Select(x => x.slot)
						.ToList();

					var booster = ActivateQueuedBooster(out var slotName, _availableSlots
						.First(slot => !usedSlots.Contains(slot)));

					Task.Run(async () =>
					{
						var channel =
							await GarbageCan.Client.GetChannelAsync(booster.slot.channelId);

						await channel.ModifyAsync(model =>
							model.Name = slotName);
					});
				}

				if (saveQueue) SaveQueue();
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		private static ActiveBooster ActivateQueuedBooster(out string slotName, AvailableSlot slot)
		{
			var queuedBooster = _queuedBoosters.Dequeue();

			var booster = new ActiveBooster
			{
				expirationDate = DateTime.Now.ToUniversalTime()
					.AddSeconds(queuedBooster.durationInSeconds),
				multiplier = queuedBooster.multiplier,
				slot = slot
			};

			ActiveBoosters.Add(booster);

			using (var context = new XpContext())
			{
				context.xpActiveBoosters.Add(new EntityActiveBooster
				{
					expiration_date = booster.expirationDate,
					multipler = booster.multiplier,
					slot = context.xpAvailableSlots.Find(booster.slot.id)
				});
				context.SaveChanges();
			}

			slotName = GetBoosterString(booster);
			return booster;
		}

		private static string GetBoosterString(ActiveBooster booster) => $"{booster.multiplier.ToString(CultureInfo.CurrentCulture)}x";

		private static void SaveQueue()
		{
			using var context = new XpContext();
			context.xpQueuedBoosters.Delete();
			var position = 0;
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
}