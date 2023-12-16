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

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<Foodorder> Foodorders { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Ordersview> Ordersviews { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Statusorder> Statusorders { get; set; }

    public virtual DbSet<Statuspayment> Statuspayments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userworkshift> Userworkshifts { get; set; }

    public virtual DbSet<Workshift> Workshifts { get; set; }

    public virtual DbSet<Workshiftview> Workshiftviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cafe;Username=postgres;Password=admin123");

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

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("pk_food");

            entity.ToTable("food");

            entity.Property(e => e.FoodId).HasColumnName("food_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<Foodorder>(entity =>
        {
            entity.HasKey(e => e.FoodorderId).HasName("pk_foodorder");

            entity.ToTable("foodorder");

            entity.Property(e => e.FoodorderId).HasColumnName("foodorder_id");
            entity.Property(e => e.FoodId).HasColumnName("food_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Food).WithMany(p => p.Foodorders)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_food");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("pk_order");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Dateorder).HasColumnName("dateorder");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.Quantityclients).HasColumnName("quantityclients");
            entity.Property(e => e.StatusorderId).HasColumnName("statusorder_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkshiftId).HasColumnName("workshift_id");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("fk_payment");

            entity.HasOne(d => d.Place).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_place");

            entity.HasOne(d => d.Statusorder).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusorderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_statusorder");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user");

            entity.HasOne(d => d.Workshift).WithMany(p => p.Orders)
                .HasForeignKey(d => d.WorkshiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_workshift");
        });

        modelBuilder.Entity<Ordersview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ordersview");

            entity.Property(e => e.Dateorder).HasColumnName("dateorder");
            entity.Property(e => e.Isnoncash).HasColumnName("isnoncash");
            entity.Property(e => e.Numberplace)
                .HasMaxLength(10)
                .HasColumnName("numberplace");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantityclients).HasColumnName("quantityclients");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkshiftId).HasColumnName("workshift_id");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("pk_payment");

            entity.ToTable("payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Datepayment).HasColumnName("datepayment");
            entity.Property(e => e.Isnoncash).HasColumnName("isnoncash");
            entity.Property(e => e.StatuspaymentId).HasColumnName("statuspayment_id");
            entity.Property(e => e.Sum).HasColumnName("sum");

            entity.HasOne(d => d.Statuspayment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatuspaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_statuspayment");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.PlaceId).HasName("pk_place");

            entity.ToTable("place");

            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.Number)
                .HasMaxLength(10)
                .HasColumnName("number");
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

        modelBuilder.Entity<Statusorder>(entity =>
        {
            entity.HasKey(e => e.StatusorderId).HasName("pk_statusorder");

            entity.ToTable("statusorder");

            entity.Property(e => e.StatusorderId).HasColumnName("statusorder_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Statuspayment>(entity =>
        {
            entity.HasKey(e => e.StatuspaymentId).HasName("pk_statuspayment");

            entity.ToTable("statuspayment");

            entity.Property(e => e.StatuspaymentId).HasColumnName("statuspayment_id");
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
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.IsFired).HasColumnName("isfired");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.SecondName)
                .HasMaxLength(100)
                .HasColumnName("secondname");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .HasColumnName("surname");

            entity.HasOne(d => d.Document).WithMany(p => p.Users)
                .HasForeignKey(d => d.DocumentId)
                .HasConstraintName("fk_document");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role");
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

        modelBuilder.Entity<Workshiftview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("workshiftview");

            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Fullname).HasColumnName("fullname");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkshiftId).HasColumnName("workshift_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
