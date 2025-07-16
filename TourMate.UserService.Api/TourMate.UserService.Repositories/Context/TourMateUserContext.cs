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

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TourGuide> TourGuides { get; set; }

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
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__F267251E4A604B6C");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__B611CB7DDC0EB30A");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
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

            entity.HasOne(d => d.Account).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Customer_Account");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__CD98462AB0EC35AB");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<TourGuide>(entity =>
        {
            entity.HasKey(e => e.TourGuideId).HasName("PK__TourGuid__D466D4A2180D8B05");

            entity.ToTable("TourGuide");

            entity.Property(e => e.TourGuideId).HasColumnName("tourGuideId");
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
            entity.Property(e => e.Company)
                .HasMaxLength(255)
                .HasColumnName("company");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.Description).HasColumnName("description");
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
            entity.Property(e => e.YearOfExperience).HasColumnName("yearOfExperience");

            entity.HasOne(d => d.Account).WithMany(p => p.TourGuides)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_TourGuide_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
