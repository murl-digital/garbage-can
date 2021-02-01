﻿// <auto-generated />
using System;
using GarbageCan.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GarbageCan.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20210110181037_KeyFixes")]
    partial class KeyFixes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("GarbageCan.Data.Entities.Boosters.EntityActiveBooster", b =>
                {
                    b.Property<int>("id")
                        .HasColumnType("int");

                    b.Property<DateTime>("expirationDate")
                        .HasColumnType("datetime");

                    b.Property<float>("multipler")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.ToTable("xp_active_boosters");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.Boosters.EntityAvailableSlot", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("channelId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("xp_available_slots");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.Boosters.EntityQueuedBooster", b =>
                {
                    b.Property<int>("position")
                        .HasColumnType("int");

                    b.Property<long>("durationInSeconds")
                        .HasColumnType("bigint");

                    b.Property<float>("multiplier")
                        .HasColumnType("float");

                    b.HasKey("position");

                    b.ToTable("xp_queued_boosters");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.Boosters.EntityUserBooster", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<long>("durationInSeconds")
                        .HasColumnType("bigint");

                    b.Property<float>("multiplier")
                        .HasColumnType("float");

                    b.Property<ulong>("userId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("xp_user_boosters");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.Moderation.EntityActionLog", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("comments")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("issuedDate")
                        .HasColumnType("datetime");

                    b.Property<ulong>("mId")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("punishmentLevel")
                        .HasColumnType("int");

                    b.Property<ulong>("uId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("moderation_warnings");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.Moderation.EntityActiveChannelRestrict", b =>
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

                    b.ToTable("moderation_active_channel_restricts");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.Moderation.EntityActiveMute", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("expirationDate")
                        .HasColumnType("datetime");

                    b.Property<ulong>("uId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("moderation_active_mutes");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.XP.EntityExcludedChannel", b =>
                {
                    b.Property<ulong>("channelId")
                        .HasColumnType("bigint unsigned");

                    b.ToTable("xp_excluded_channels");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.XP.EntityLevelRole", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("lvl")
                        .HasColumnType("int");

                    b.Property<bool>("remain")
                        .HasColumnType("tinyint(1)");

                    b.Property<ulong>("roleId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("xp_level_roles");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.XP.EntityUser", b =>
                {
                    b.Property<ulong>("id")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("lvl")
                        .HasColumnType("int");

                    b.Property<double>("xp")
                        .HasColumnType("double");

                    b.HasKey("id");

                    b.ToTable("xp_users");
                });

            modelBuilder.Entity("GarbageCan.Data.Entities.Boosters.EntityActiveBooster", b =>
                {
                    b.HasOne("GarbageCan.Data.Entities.Boosters.EntityAvailableSlot", "slot")
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
