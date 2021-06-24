﻿// <auto-generated />
using System;
using CWE.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CWE.Data.Migrations
{
    [DbContext(typeof(CWEDbContext))]
    partial class CWEDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("CWE.Data.Models.Campaign", b =>
                {
                    b.Property<ulong>("User")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("Initiator")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Message")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("Minimal")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("User");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("CWE.Data.Models.Infraction", b =>
                {
                    b.Property<Guid>("InfractionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext");

                    b.Property<ulong>("StaffId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("StaffUsername")
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Username")
                        .HasColumnType("longtext");

                    b.HasKey("InfractionId");

                    b.ToTable("Infractions");
                });

            modelBuilder.Entity("CWE.Data.Models.Mute", b =>
                {
                    b.Property<Guid>("InfractionId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("MuteEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("MuteStart")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("User")
                        .HasColumnType("bigint unsigned");

                    b.ToTable("Mutes");
            modelBuilder.Entity("CWE.Data.Models.Rank", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("Ranks");
                });

            modelBuilder.Entity("CWE.Data.Models.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<ulong>("Initiator")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("CWE.Data.Models.Suggestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("Initiator")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Suggestions");
                });

            modelBuilder.Entity("CWE.Data.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<ulong>("OwnerId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
