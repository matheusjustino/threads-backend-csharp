﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ThreadsBackend.Api.Infrastructure.Persistence;

#nullable disable

namespace ThreadsBackend.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.Community", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Communities");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.CommunityMember", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CommunityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MemberId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("MemberId");

                    b.ToTable("CommunityMembers");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.Thread", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CommunityId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ParentThreadId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CommunityId");

                    b.HasIndex("ParentThreadId");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Onboarded")
                        .HasColumnType("boolean");

                    b.Property<string>("ProfilePhoto")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.Community", b =>
                {
                    b.HasOne("ThreadsBackend.Api.Domain.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.CommunityMember", b =>
                {
                    b.HasOne("ThreadsBackend.Api.Domain.Entities.Community", "Community")
                        .WithMany("Members")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ThreadsBackend.Api.Domain.Entities.User", "Member")
                        .WithMany("Communities")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.Thread", b =>
                {
                    b.HasOne("ThreadsBackend.Api.Domain.Entities.User", "Author")
                        .WithMany("Threads")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ThreadsBackend.Api.Domain.Entities.Community", "Community")
                        .WithMany("Threads")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ThreadsBackend.Api.Domain.Entities.Thread", null)
                        .WithMany("Comments")
                        .HasForeignKey("ParentThreadId");

                    b.Navigation("Author");

                    b.Navigation("Community");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.Community", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("Threads");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.Thread", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("ThreadsBackend.Api.Domain.Entities.User", b =>
                {
                    b.Navigation("Communities");

                    b.Navigation("Threads");
                });
#pragma warning restore 612, 618
        }
    }
}
