﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenBreweryDB.Data;

namespace OpenBreweryDB.Data.Migrations
{
    [DbContext(typeof(BreweryDbContext))]
    [Migration("20200629114917_AddBreweryIdIndex")]
    partial class AddBreweryIdIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4");

            modelBuilder.Entity("OpenBreweryDB.Data.Models.Brewery", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BreweryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BreweryType")
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Latitude")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Longitude")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.Property<string>("PostalCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .HasColumnType("TEXT");

                    b.Property<string>("Street")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("WebsiteURL")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("breweries");
                });

            modelBuilder.Entity("OpenBreweryDB.Data.Models.BreweryTag", b =>
                {
                    b.Property<long>("BreweryId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Id")
                        .HasColumnType("INTEGER");

                    b.HasKey("BreweryId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("BreweryTags");
                });

            modelBuilder.Entity("OpenBreweryDB.Data.Models.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasName("Index_Tags_On_Name");

                    b.ToTable("tags");
                });

            modelBuilder.Entity("OpenBreweryDB.Data.Models.BreweryTag", b =>
                {
                    b.HasOne("OpenBreweryDB.Data.Models.Brewery", "Brewery")
                        .WithMany("BreweryTags")
                        .HasForeignKey("BreweryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OpenBreweryDB.Data.Models.Tag", "Tag")
                        .WithMany("BreweryTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
