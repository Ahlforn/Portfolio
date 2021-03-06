﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PriceCalculator.Models;

namespace PriceCalculator.Migrations
{
    [DbContext(typeof(SiteContext))]
    [Migration("20191203210511_V7")]
    partial class V7
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("PriceCalculator.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("CVR");

                    b.Property<string>("City");

                    b.Property<string>("ContactEmail");

                    b.Property<string>("ContactName");

                    b.Property<string>("ContactPhoneNumber");

                    b.Property<string>("Country");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("ZIPCode");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("PriceCalculator.Models.LayerThickness", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ExcelDefinedName");

                    b.Property<int>("MaterialID");

                    b.Property<double>("Thickness");

                    b.Property<string>("Unit");

                    b.HasKey("ID");

                    b.HasIndex("MaterialID");

                    b.ToTable("LayerThicknesses");
                });

            modelBuilder.Entity("PriceCalculator.Models.Material", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("Materials");
                });

            modelBuilder.Entity("PriceCalculator.Models.MaterialPostProcess", b =>
                {
                    b.Property<int>("MaterialID");

                    b.Property<int>("PostProcessID");

                    b.HasKey("MaterialID", "PostProcessID");

                    b.HasIndex("PostProcessID");

                    b.ToTable("MaterialPostProcesses");
                });

            modelBuilder.Entity("PriceCalculator.Models.PostProcess", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("ExcelDefinedName");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("PostProcesses");
                });

            modelBuilder.Entity("PriceCalculator.Models.Printer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Printers");
                });

            modelBuilder.Entity("PriceCalculator.Models.PrinterLayerThickness", b =>
                {
                    b.Property<int>("PrinterID");

                    b.Property<int>("LayerThicknessID");

                    b.HasKey("PrinterID", "LayerThicknessID");

                    b.HasIndex("LayerThicknessID");

                    b.ToTable("PrinterLayerThickness");
                });

            modelBuilder.Entity("PriceCalculator.Models.PrintModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<string>("Description");

                    b.Property<string>("Filename");

                    b.Property<int?>("LayerThicknessID");

                    b.Property<int?>("MaterialID");

                    b.Property<string>("Name");

                    b.Property<double>("PricePerPart");

                    b.Property<double>("PriceTotal");

                    b.Property<string>("PrintDirection");

                    b.Property<int?>("QuotationID");

                    b.Property<string>("Snapshot");

                    b.Property<DateTime>("UploadDate");

                    b.Property<string>("UserId");

                    b.Property<double>("Volume");

                    b.Property<double>("X");

                    b.Property<double>("Y");

                    b.Property<double>("Z");

                    b.HasKey("ID");

                    b.HasIndex("LayerThicknessID");

                    b.HasIndex("MaterialID");

                    b.HasIndex("QuotationID");

                    b.HasIndex("UserId");

                    b.ToTable("PrintModels");
                });

            modelBuilder.Entity("PriceCalculator.Models.PrintModelPostProcess", b =>
                {
                    b.Property<int>("PrintModelID");

                    b.Property<int>("PostProcessID");

                    b.HasKey("PrintModelID", "PostProcessID");

                    b.HasIndex("PostProcessID");

                    b.ToTable("PrintModelPostProcesses");
                });

            modelBuilder.Entity("PriceCalculator.Models.Quotation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContactEmail");

                    b.Property<string>("ContactName");

                    b.Property<string>("ContactPhoneNumber");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CustomerAddress");

                    b.Property<string>("CustomerName");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("Quotations");
                });

            modelBuilder.Entity("PriceCalculator.Models.SiteUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<int?>("CompanyId");

                    b.Property<string>("Department");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<int?>("PhoneCountryCode");

                    b.Property<string>("Title");

                    b.HasIndex("CompanyId");

                    b.ToTable("SiteUser");

                    b.HasDiscriminator().HasValue("SiteUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PriceCalculator.Models.LayerThickness", b =>
                {
                    b.HasOne("PriceCalculator.Models.Material", "Material")
                        .WithMany("LayerThicknesses")
                        .HasForeignKey("MaterialID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PriceCalculator.Models.MaterialPostProcess", b =>
                {
                    b.HasOne("PriceCalculator.Models.Material", "Material")
                        .WithMany("MaterialPostProcesses")
                        .HasForeignKey("MaterialID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PriceCalculator.Models.PostProcess", "PostProcess")
                        .WithMany("MaterialPostProcesses")
                        .HasForeignKey("PostProcessID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PriceCalculator.Models.PrinterLayerThickness", b =>
                {
                    b.HasOne("PriceCalculator.Models.LayerThickness", "LayerThickness")
                        .WithMany("PrinterLayerThicknesses")
                        .HasForeignKey("LayerThicknessID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PriceCalculator.Models.Printer", "Printer")
                        .WithMany("PrinterLayerThicknesses")
                        .HasForeignKey("PrinterID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PriceCalculator.Models.PrintModel", b =>
                {
                    b.HasOne("PriceCalculator.Models.LayerThickness", "LayerThickness")
                        .WithMany()
                        .HasForeignKey("LayerThicknessID");

                    b.HasOne("PriceCalculator.Models.Material", "Material")
                        .WithMany()
                        .HasForeignKey("MaterialID");

                    b.HasOne("PriceCalculator.Models.Quotation", "Quotation")
                        .WithMany("PrintModels")
                        .HasForeignKey("QuotationID");

                    b.HasOne("PriceCalculator.Models.SiteUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("PriceCalculator.Models.PrintModelPostProcess", b =>
                {
                    b.HasOne("PriceCalculator.Models.PostProcess", "PostProcess")
                        .WithMany("PrintModelPostProcesses")
                        .HasForeignKey("PostProcessID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PriceCalculator.Models.PrintModel", "PrintModel")
                        .WithMany("PrintModelPostProcesses")
                        .HasForeignKey("PrintModelID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PriceCalculator.Models.Quotation", b =>
                {
                    b.HasOne("PriceCalculator.Models.SiteUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("PriceCalculator.Models.SiteUser", b =>
                {
                    b.HasOne("PriceCalculator.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });
#pragma warning restore 612, 618
        }
    }
}
