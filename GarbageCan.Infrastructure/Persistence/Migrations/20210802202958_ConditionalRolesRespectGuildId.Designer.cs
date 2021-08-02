﻿// <auto-generated />
using System;
using GarbageCan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GarbageCan.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210802202958_ConditionalRolesRespectGuildId")]
    partial class ConditionalRolesRespectGuildId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("GarbageCan.Domain.Entities.Boosters.ActiveBooster", b =>
                {
                    b.Property<int>("id")
                        .HasColumnType("int");

                    b.Property<DateTime>("expirationDate")
                        .HasColumnType("datetime");

                    b.Property<float>("multipler")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.ToTable("xpActiveBoosters");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Boosters.AvailableSlot", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("channelId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("xpAvailableSlots");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Boosters.QueuedBooster", b =>
                {
                    b.Property<int>("position")
                        .HasColumnType("int");

                    b.Property<long>("durationInSeconds")
                        .HasColumnType("bigint");

                    b.Property<float>("multiplier")
                        .HasColumnType("float");

                    b.HasKey("position");

                    b.ToTable("xpQueuedBoosters");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Boosters.UserBooster", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("durationInSeconds")
                        .HasColumnType("bigint");

                    b.Property<float>("multiplier")
                        .HasColumnType("float");

                    b.Property<ulong>("userId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("xpUserBoosters");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Config.Config", b =>
                {
                    b.Property<string>("key")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("value")
                        .HasColumnType("longtext");

                    b.HasKey("key");

                    b.ToTable("config");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Moderation.ActionLog", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("comments")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("issuedDate")
                        .HasColumnType("datetime");

                    b.Property<ulong>("mId")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("punishmentLevel")
                        .HasColumnType("int");

                    b.Property<ulong>("uId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("moderationActionLogs");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Moderation.ActiveChannelRestrict", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("channelId")
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("expirationDate")
                        .HasColumnType("datetime");

                    b.Property<ulong>("uId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("moderationActiveChannelRestricts");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Moderation.ActiveMute", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("expirationDate")
                        .HasColumnType("datetime");

                    b.Property<ulong>("uId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("moderationActiveMutes");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Roles.ConditionalRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("guildId");

                    b.Property<bool>("Remain")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("remain");

                    b.Property<ulong>("RequiredRoleId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("requiredRoleId");

                    b.Property<ulong>("ResultRoleId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("resultRoleId");

                    b.HasKey("Id");

                    b.ToTable("conditionalRoles");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Roles.JoinRole", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("roleId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("joinRoles");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Roles.LevelRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("guildId");

                    b.Property<int>("Lvl")
                        .HasColumnType("int")
                        .HasColumnName("lvl");

                    b.Property<bool>("Remain")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("remain");

                    b.Property<ulong>("RoleId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("roleId");

                    b.HasKey("Id");

                    b.ToTable("levelRoles");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Roles.ReactionRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("channelId");

                    b.Property<string>("EmoteId")
                        .HasColumnType("longtext")
                        .HasColumnName("emoteId");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("guildId");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("messageId");

                    b.Property<ulong>("RoleId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("roleId");

                    b.HasKey("Id");

                    b.ToTable("reactionRoles");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Roles.WatchedUser", b =>
                {
                    b.Property<ulong>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("joinWatchlist");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.XP.ExcludedChannel", b =>
                {
                    b.Property<ulong>("ChannelId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("channelId");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("guildId");

                    b.ToTable("xpExcludedChannels");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.XP.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("guildId");

                    b.Property<int>("Lvl")
                        .HasColumnType("int")
                        .HasColumnName("lvl");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned")
                        .HasColumnName("userId");

                    b.Property<double>("XP")
                        .HasColumnType("double")
                        .HasColumnName("xp");

                    b.HasKey("Id");

                    b.ToTable("xpUsers");
                });

            modelBuilder.Entity("GarbageCan.Domain.Entities.Boosters.ActiveBooster", b =>
                {
                    b.HasOne("GarbageCan.Domain.Entities.Boosters.AvailableSlot", "slot")
                        .WithMany()
                        .HasForeignKey("id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("slot");
                });
#pragma warning restore 612, 618
        }
    }
}
