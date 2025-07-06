using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.Context;

public partial class TourMateUserContext : DbContext
{
    public TourMateUserContext()
    {
    }

    public TourMateUserContext(DbContextOptions<TourMateUserContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<TourGuide> TourGuides { get; set; }

    public virtual DbSet<TourGuideDesc> TourGuideDescs { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Customer");

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.CustomerId)
                .ValueGeneratedOnAdd()
                .HasColumnName("customerId");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasColumnName("fullName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<TourGuide>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TourGuide");

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .HasColumnName("bankAccountNumber");
            entity.Property(e => e.BankName)
                .HasMaxLength(50)
                .HasColumnName("bankName");
            entity.Property(e => e.BannerImage).HasColumnName("bannerImage");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasColumnName("fullName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.IsVerified).HasColumnName("isVerified");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.TourGuideId)
                .ValueGeneratedOnAdd()
                .HasColumnName("tourGuideId");
        });

        modelBuilder.Entity<TourGuideDesc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TourGuideDesc");

            entity.Property(e => e.AreaId).HasColumnName("areaId");
            entity.Property(e => e.Company)
                .HasMaxLength(255)
                .HasColumnName("company");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.TourGuideDescId)
                .ValueGeneratedOnAdd()
                .HasColumnName("tourGuideDescId");
            entity.Property(e => e.TourGuideId).HasColumnName("tourGuideId");
            entity.Property(e => e.YearOfExperience).HasColumnName("yearOfExperience");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
