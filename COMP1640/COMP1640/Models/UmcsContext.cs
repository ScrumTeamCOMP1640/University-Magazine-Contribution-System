using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

public partial class UmcsContext : DbContext
{
    public UmcsContext()
    {
    }

    public UmcsContext(DbContextOptions<UmcsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<FaU> FaUs { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SelectedArticle> SelectedArticles { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<TermsAndCondition> TermsAndConditions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=MSI\\SQLEXPRESS;Initial Catalog=UMCS;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270C8C9C53520");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubmissionDate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Articles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Articles__UserID__5535A963");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA2F21338E");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.CommentContent).HasColumnType("text");
            entity.Property(e => e.CommentDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Article).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__Comments__Articl__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__UserID__59063A47");
        });

        modelBuilder.Entity<FaU>(entity =>
        {
            entity.HasKey(e => e.FauId).HasName("PK__FaU__95570A4EBC1DFAD8");

            entity.ToTable("FaU");

            entity.Property(e => e.FauId).HasColumnName("FauID");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Faculty).WithMany(p => p.FaUs)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FaU_Faculties");

            entity.HasOne(d => d.User).WithMany(p => p.FaUs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FaU_Users");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__Facultie__306F636E93990DAB");

            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.FacultyName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A09690D87");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616039DDC28F").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SelectedArticle>(entity =>
        {
            entity.HasKey(e => e.SelectedArticleId).HasName("PK__Selected__80A5E4B48DB88ADB");

            entity.Property(e => e.SelectedArticleId).HasColumnName("SelectedArticleID");
            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");

            entity.HasOne(d => d.Article).WithMany(p => p.SelectedArticles)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__SelectedA__Artic__60A75C0F");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301BDB0BC499F");

            entity.ToTable("Semester");

            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.ClosureDate).HasColumnType("datetime");
            entity.Property(e => e.SemesterYear)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Article).WithMany(p => p.Semesters)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK_Semester_Articles");
        });

        modelBuilder.Entity<TermsAndCondition>(entity =>
        {
            entity.HasKey(e => e.TermsAndConditionsId).HasName("PK__TermsAnd__65787321F7CE0CA3");

            entity.Property(e => e.TermsAndConditionsId).HasColumnName("TermsAndConditionsID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.Version)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC540AAE9E");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
