﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VinylShop.Data;

#nullable disable

namespace MyWebApp.Migrations
{
    [DbContext(typeof(VinylShopContext))]
    partial class VinylShopContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("BuyerId")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DeliveryMethod")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Packaging")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PromoCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("VinylShop.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ArtistId")
                        .HasColumnType("int");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<string>("ImagePath")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("GenreId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("VinylShop.Models.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Biography")
                        .HasMaxLength(3000)
                        .HasColumnType("varchar(3000)");

                    b.Property<string>("ImagePath")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("VinylShop.Models.Buyer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Buyers");
                });

            modelBuilder.Entity("VinylShop.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("VinylShop.Models.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time(6)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("Order", b =>
                {
                    b.HasOne("VinylShop.Models.Buyer", "Buyer")
                        .WithMany("Orders")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Buyer");
                });

            modelBuilder.Entity("OrderItem", b =>
                {
                    b.HasOne("VinylShop.Models.Album", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("VinylShop.Models.Album", b =>
                {
                    b.HasOne("VinylShop.Models.Artist", "Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VinylShop.Models.Genre", "Genre")
                        .WithMany("Albums")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("VinylShop.Models.Song", b =>
                {
                    b.HasOne("VinylShop.Models.Album", "Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");
                });

            modelBuilder.Entity("Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("VinylShop.Models.Album", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("VinylShop.Models.Artist", b =>
                {
                    b.Navigation("Albums");
                });

            modelBuilder.Entity("VinylShop.Models.Buyer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("VinylShop.Models.Genre", b =>
                {
                    b.Navigation("Albums");
                });
#pragma warning restore 612, 618
        }
    }
}
