using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace caffeApp.Desktop;

public partial class CafeContext : DbContext
{
    public CafeContext()
    {
    }

    public CafeContext(DbContextOptions<CafeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userworkshift> Userworkshifts { get; set; }

    public virtual DbSet<Workshift> Workshifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cafe;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("pk_document");

            entity.ToTable("document");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.Contractlink).HasColumnName("contractlink");
            entity.Property(e => e.Photolink).HasColumnName("photolink");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("pk_role");

            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("pk_user");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Isfired)
                .HasDefaultValueSql("false")
                .HasColumnName("isfired");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Secondname)
                .HasMaxLength(100)
                .HasColumnName("secondname");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .HasColumnName("surname");

            entity.HasOne(d => d.Document).WithMany(p => p.Users)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_1");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_2");
        });

        modelBuilder.Entity<Userworkshift>(entity =>
        {
            entity.HasKey(e => e.UserworkshiftId).HasName("pk_userworkshift");

            entity.ToTable("userworkshift");

            entity.Property(e => e.UserworkshiftId).HasColumnName("userworkshift_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkshiftId).HasColumnName("workshift_id");

            entity.HasOne(d => d.User).WithMany(p => p.Userworkshifts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user");

            entity.HasOne(d => d.Workshift).WithMany(p => p.Userworkshifts)
                .HasForeignKey(d => d.WorkshiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_workshift");
        });

        modelBuilder.Entity<Workshift>(entity =>
        {
            entity.HasKey(e => e.WorkshiftId).HasName("pk_workshift");

            entity.ToTable("workshift");

            entity.Property(e => e.WorkshiftId).HasColumnName("workshift_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Timeend).HasColumnName("timeend");
            entity.Property(e => e.Timestart).HasColumnName("timestart");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
