﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReversiMvcApp.Data;

#nullable disable

namespace ReversiMvcApp.Migrations
{
    [DbContext(typeof(ReversiDbContext))]
    [Migration("20230123083057_SpelerMigration")]
    partial class SpelerMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.5");

            modelBuilder.Entity("ReversiMvcApp.Models.Speler", b =>
                {
                    b.Property<string>("Guuid")
                        .HasColumnType("TEXT");

                    b.Property<int>("AantalGelijk")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AantalGewonnen")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AantalVerloren")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Naam")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Guuid");

                    b.ToTable("Spelers");
                });
#pragma warning restore 612, 618
        }
    }
}