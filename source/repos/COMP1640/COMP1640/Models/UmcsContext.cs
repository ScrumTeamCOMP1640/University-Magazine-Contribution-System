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

    public virtual DbSet<ClosureDate> ClosureDates { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<GuestAccount> GuestAccounts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SelectedArticle> SelectedArticles { get; set; }

    public virtual DbSet<TermsAndCondition> TermsAndConditions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MSI\\SQLEXPRESS;Initial Catalog=UMCS;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270C8A2A00CF8");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
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

            entity.HasOne(d => d.Faculty).WithMany(p => p.Articles)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__Articles__Facult__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.Articles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Articles__UserID__5535A963");
        });

        modelBuilder.Entity<ClosureDate>(entity =>
        {
            entity.HasKey(e => e.ClosureDateId).HasName("PK__ClosureD__6A31BA0EBA1E4644");

            entity.Property(e => e.ClosureDateId).HasColumnName("ClosureDateID");
            entity.Property(e => e.ClosureDate1)
                .HasColumnType("datetime")
                .HasColumnName("ClosureDate");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");

            entity.HasOne(d => d.Faculty).WithMany(p => p.ClosureDates)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__ClosureDa__Facul__5BE2A6F2");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAAB5A3A20C");

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

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__Facultie__306F636E42D8A416");

            entity.HasIndex(e => e.FacultyName, "UQ__Facultie__BFD889E199F0537C").IsUnique();

            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.FacultyName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GuestAccount>(entity =>
        {
            entity.HasKey(e => e.GuestId).HasName("PK__GuestAcc__0C423C324D01EC0E");

            entity.HasIndex(e => e.Username, "UQ__GuestAcc__536C85E442CE9C82").IsUnique();

            entity.Property(e => e.GuestId).HasColumnName("GuestID");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Faculty).WithMany(p => p.GuestAccounts)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__GuestAcco__Facul__6477ECF3");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A8639E0E9");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616037FBE747").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SelectedArticle>(entity =>
        {
            entity.HasKey(e => e.SelectedArticleId).HasName("PK__Selected__80A5E4B4A2E08D54");

            entity.Property(e => e.SelectedArticleId).HasColumnName("SelectedArticleID");
            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");

            entity.HasOne(d => d.Article).WithMany(p => p.SelectedArticles)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__SelectedA__Artic__60A75C0F");
        });

        modelBuilder.Entity<TermsAndCondition>(entity =>
        {
            entity.HasKey(e => e.TermsAndConditionsId).HasName("PK__TermsAnd__65787321D65554CF");

            entity.Property(e => e.TermsAndConditionsId).HasColumnName("TermsAndConditionsID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.Version)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC37EB43DB");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E41DD70A7B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534AF363BAC").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
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
