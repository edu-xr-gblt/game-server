﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GBLT.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230615034534_Add-UserName")]
    partial class AddUserName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Entity.TMeta", b =>
                {
                    b.Property<int>("EId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EId"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("MetaKey")
                        .HasColumnType("text");

                    b.Property<string>("MetaValue")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("EId");

                    b.ToTable("Meta");
                });

            modelBuilder.Entity("Core.Entity.TQuiz", b =>
                {
                    b.Property<int>("EId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EId"));

                    b.Property<string[]>("Answers")
                        .HasColumnType("text[]");

                    b.Property<int>("CorrectIdx")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Quetsion")
                        .HasColumnType("text");

                    b.Property<int?>("TQuizCollectionEId")
                        .HasColumnType("integer");

                    b.Property<string>("ThumbNail")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("EId");

                    b.HasIndex("TQuizCollectionEId");

                    b.ToTable("Quiz");
                });

            modelBuilder.Entity("Core.Entity.TQuizCollection", b =>
                {
                    b.Property<int>("EId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EId"));

                    b.Property<string>("Configuration")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("OwnerEId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("EId");

                    b.HasIndex("OwnerEId");

                    b.ToTable("QuizCollection");
                });

            modelBuilder.Entity("Core.Entity.TRefreshToken", b =>
                {
                    b.Property<int>("EId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EId"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("RemoteIpAddress")
                        .HasColumnType("text");

                    b.Property<int?>("TUserEId")
                        .HasColumnType("integer");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("EId");

                    b.HasIndex("TUserEId");

                    b.ToTable("TRefreshToken");
                });

            modelBuilder.Entity("Core.Entity.TUser", b =>
                {
                    b.Property<int>("EId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EId"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("IdentityId")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("EId");

                    b.HasIndex("Username");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Core.Entity.TQuiz", b =>
                {
                    b.HasOne("Core.Entity.TQuizCollection", null)
                        .WithMany("Quizzes")
                        .HasForeignKey("TQuizCollectionEId");
                });

            modelBuilder.Entity("Core.Entity.TQuizCollection", b =>
                {
                    b.HasOne("Core.Entity.TUser", "Owner")
                        .WithMany("QuizCollections")
                        .HasForeignKey("OwnerEId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Core.Entity.TRefreshToken", b =>
                {
                    b.HasOne("Core.Entity.TUser", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("TUserEId");
                });

            modelBuilder.Entity("Core.Entity.TQuizCollection", b =>
                {
                    b.Navigation("Quizzes");
                });

            modelBuilder.Entity("Core.Entity.TUser", b =>
                {
                    b.Navigation("QuizCollections");

                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}