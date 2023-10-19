﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetCa.Infrastructure.Persistence;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NetCa.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230715131708_InitDb")]
    partial class InitDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("NetCa.Domain.Entities.Changelog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ChangeBy")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTime>("ChangeDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("KeyValues")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Method")
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)");

                    b.Property<string>("NewValues")
                        .HasColumnType("text");

                    b.Property<string>("OldValues")
                        .HasColumnType("text");

                    b.Property<string>("TableName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Changelogs");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.MessageBroker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Acknowledged")
                        .HasColumnType("bool");

                    b.Property<bool>("IsSend")
                        .HasColumnType("bool");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<DateTime?>("StoredDate")
                        .HasColumnType("Timestamp");

                    b.Property<string>("Topic")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("MessageBroker");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.ReceivedMessageBroker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<string>("InnerMessage")
                        .HasColumnType("text");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<long>("Offset")
                        .HasColumnType("bigint");

                    b.Property<int>("Partition")
                        .HasColumnType("int");

                    b.Property<string>("StackTrace")
                        .HasColumnType("text");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeFinish")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("TimeIn")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("TimeProcess")
                        .HasColumnType("timestamp");

                    b.Property<string>("Topic")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("ReceivedMessageBroker");
                });
#pragma warning restore 612, 618
        }
    }
}
