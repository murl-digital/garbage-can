using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Boosters;
using GarbageCan.Data.Models;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.XP.Boosters
{
	public class BoosterManager : IFeature
	{
		private static List<AvailableSlot> _availableSlots;
		private static readonly List<ActiveBooster> ActiveBoosters = new();
		private static Queue<QueuedBooster> _queuedBoosters;

		private static readonly Timer BoosterTimer = new(5000);

		public void Init(DiscordClient client)
		{
			using (var context = new Context())
			{
				_availableSlots = context.xpAvailableSlots
					.Select(slot => new AvailableSlot {channelId = slot.channel_id, id = slot.id})
					.ToList();
			}

			client.Ready += (_, _) =>
			{
				Task.Run(Ready);

				BoosterTimer.Elapsed += Tick;

				BoosterTimer.Enabled = true;

				return Task.CompletedTask;
			};

			client.GuildUpdated += (_, args) =>
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
			using var context = new Context();
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

			ActivateBooster(multiplier, duration, slot);
		}

		private static void Ready()
		{
			try
			{
				using var context = new Context();

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

				using (var context = new Context())
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

				var saveQueue = false;
				
				toProcess.ForEach(b =>
				{
					ActiveBoosters.Remove(b);

					if (_queuedBoosters.Count > 0)
					{
						saveQueue = true;
						ActivateQueuedBooster(b.slot);
					}
					else
					{
						Task.Run(async () =>
						{
							var channel = await GarbageCan.Client.GetChannelAsync(b.slot.channelId);
							await channel.ModifyAsync(model => model.Name = "-");
						});
					}
				});

				while (_queuedBoosters.Count > 0 && ActiveBoosters.Count < _availableSlots.Count)
				{
					saveQueue = true;

					var usedSlots = ActiveBoosters
						.Select(b => b.slot)
						.ToList();

					var slot = _availableSlots
						.First(s => !usedSlots.Contains(s));
					
					ActivateQueuedBooster(slot);
				}

				if (saveQueue) SaveQueue();
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		private static void ActivateQueuedBooster(AvailableSlot slot)
		{
			var booster = _queuedBoosters.Dequeue();
			ActivateBooster(booster.multiplier, TimeSpan.FromSeconds(booster.durationInSeconds), slot);
		}

		private static void ActivateBooster(float multiplier, TimeSpan duration, AvailableSlot slot)
		{
			var booster = new ActiveBooster
			{
				expirationDate = DateTime.Now.ToUniversalTime().Add(duration),
				multiplier = multiplier,
				slot = slot
			};

			ActiveBoosters.Add(booster);

			using (var context = new Context())
			{
				context.xpActiveBoosters.Add(new EntityActiveBooster
				{
					expiration_date = booster.expirationDate,
					multipler = booster.multiplier,
					slot = context.xpAvailableSlots.Find(booster.slot.id)
				});
				context.SaveChanges();
			}

			Task.Run(async () =>
			{
				var channel = await GarbageCan.Client.GetChannelAsync(booster.slot.channelId);
				await channel.ModifyAsync(model => model.Name = GetBoosterString(booster));
			});
		}

		private static string GetBoosterString(Booster booster) => $"{booster.multiplier.ToString(CultureInfo.CurrentCulture)}x";

		private static void SaveQueue()
		{
			using var context = new Context();
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