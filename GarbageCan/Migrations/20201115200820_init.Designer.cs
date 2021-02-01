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
    [Migration("20201115200820_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("GarbageCan.Data.XPActiveBooster", b =>
                {
                    b.Property<int>("slot_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("expiration_date")
                        .HasColumnType("datetime");

                    b.Property<float>("multipler")
                        .HasColumnType("float");

                    b.Property<int?>("slotid")
                        .HasColumnType("int");

                    b.HasKey("slot_id");

                    b.HasIndex("slotid");

                    b.ToTable("xp_active_boosters");
                });

            modelBuilder.Entity("GarbageCan.Data.XPAvailableSlot", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("channel_id")
                        .HasMaxLength(18)
                        .HasColumnType("varchar(18) CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("xp_available_slots");
                });

            modelBuilder.Entity("GarbageCan.Data.XPQueuedBooster", b =>
                {
                    b.Property<int>("position")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("duration_in_seconds")
                        .HasColumnType("bigint");

                    b.Property<float>("multiplier")
                        .HasColumnType("float");

                    b.HasKey("position");

                    b.ToTable("xp_queued_boosters");
                });

            modelBuilder.Entity("GarbageCan.Data.XPUser", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(18)
                        .HasColumnType("varchar(18) CHARACTER SET utf8mb4");

                    b.Property<int>("lvl")
                        .HasColumnType("int");

                    b.Property<double>("xp")
                        .HasColumnType("double");

                    b.HasKey("id");

                    b.ToTable("xp_users");
                });

            modelBuilder.Entity("GarbageCan.Data.XPUserBooster", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<long>("duration_in_seconds")
                        .HasColumnType("bigint");

                    b.Property<float>("multiplier")
                        .HasColumnType("float");

                    b.Property<string>("user_id")
                        .HasMaxLength(18)
                        .HasColumnType("varchar(18) CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("xp_user_boosters");
                });

            modelBuilder.Entity("GarbageCan.Data.XPActiveBooster", b =>
                {
                    b.HasOne("GarbageCan.Data.XPAvailableSlot", "slot")
                        .WithMany()
                        .HasForeignKey("slotid");

                    b.Navigation("slot");
                });
#pragma warning restore 612, 618
        }
    }
}
