﻿// <auto-generated />
using System;
using EmailTamer.Database.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmailTamer.Database.Tenant.Migrations
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20250204120750_AddUserName")]
    partial class AddUserName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EmailTamer.Database.Tenant.Entities.EmailBox", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)");

                    b.Property<bool>("AuthenticateByEmail")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("BoxName")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasPrecision(0)
                        .HasColumnType("DATETIME");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmailDomainConnectionHost")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("EmailDomainConnectionPort")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastSyncAt")
                        .HasPrecision(0)
                        .HasColumnType("DATETIME");

                    b.Property<DateTime>("ModifiedAt")
                        .HasPrecision(0)
                        .HasColumnType("DATETIME");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("UseSSl")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt", "ModifiedAt");

                    b.ToTable("EmailBoxes", t =>
                        {
                            t.HasCheckConstraint("CHK_EmailBoxes_IdNotDefault", "\"Id\" <> '00000000-0000-0000-0000-000000000000'");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
