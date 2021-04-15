﻿// <auto-generated />
using System;
using Kanban.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kanban.DAL.Migrations
{
    [DbContext(typeof(KanbanDbContext))]
    [Migration("20210415100854_dateAdded")]
    partial class dateAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kanban.DAL.Models.TaskColumn", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("TaskColumns");
                });

            modelBuilder.Entity("Kanban.DAL.Models.TaskModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DeadLine")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("State")
                        .HasColumnType("int");

                    b.Property<int?>("TaskColumnID")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TaskColumnID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Kanban.DAL.Models.TaskModel", b =>
                {
                    b.HasOne("Kanban.DAL.Models.TaskColumn", "TaskColumn")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskColumnID");

                    b.Navigation("TaskColumn");
                });

            modelBuilder.Entity("Kanban.DAL.Models.TaskColumn", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
