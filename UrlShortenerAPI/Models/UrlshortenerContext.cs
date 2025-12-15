using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UrlShortenerAPI.Models;

public partial class UrlshortenerContext : DbContext
{
    public UrlshortenerContext()
    {
    }

    public UrlshortenerContext(DbContextOptions<UrlshortenerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Click> Clicks { get; set; }

    public virtual DbSet<Url> Urls { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Click>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clicks__3214EC07D7631DC0");

            entity.HasIndex(e => e.ClickedAt, "IX_Clicks_ClickedAt");

            entity.HasIndex(e => e.UrlId, "IX_Clicks_ShortUrlId");

            entity.Property(e => e.ClickedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Referrer).HasMaxLength(2048);
            entity.Property(e => e.UserAgent).HasMaxLength(512);

            entity.HasOne(d => d.Url).WithMany(p => p.Clicks)
                .HasForeignKey(d => d.UrlId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clicks_ShortUrls");
        });

        modelBuilder.Entity<Url>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Urls__3214EC07152A9AFB");

            entity.HasIndex(e => e.UrlCode, "IX_ShortUrls_ShortCode").IsUnique();

            entity.HasIndex(e => e.UserId, "IX_ShortUrls_UserId");

            entity.HasIndex(e => e.UrlCode, "UQ__Urls__76E6BB828FEFCC26").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.OriginalUrl).HasMaxLength(2048);
            entity.Property(e => e.UrlCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Urls)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ShortUrls_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC072D75E508");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E98A252F").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(60);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_UserTypes");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserType__3214EC079C1A1A1F");

            entity.HasIndex(e => e.Name, "UQ__UserType__737584F6B18B29B3").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
