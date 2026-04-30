using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ModifiedProject.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admission> Admissions { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Enquiry> Enquiries { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentErrror> PaymentErrrors { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<SalesRequest> SalesRequests { get; set; }

    public virtual DbSet<SpookyRequest> SpookyRequests { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admission>(entity =>
        {
            entity.HasIndex(e => e.CourseId, "IX_Admissions_CourseID");

            entity.HasIndex(e => e.StudentId, "IX_Admissions_StudentId");

            entity.Property(e => e.AdmissionId).HasColumnName("AdmissionID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Course).WithMany(p => p.Admissions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Student).WithMany(p => p.Admissions).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Courses_UserId");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Duration).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Courses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");
        });

        modelBuilder.Entity<Enquiry>(entity =>
        {
            entity.HasIndex(e => e.CourseId, "IX_Enquiries_CourseID");

            entity.HasIndex(e => e.StudentId, "IX_Enquiries_StudentId");

            entity.Property(e => e.EnquiryId).HasColumnName("EnquiryID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EnquiryType).HasMaxLength(100);
            entity.Property(e => e.ReplyMessage).HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Open");
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Course).WithMany(p => p.Enquiries)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Student).WithMany(p => p.Enquiries).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.AdmissionId, "IX_Payments_AdmissionID");

            entity.HasIndex(e => e.CourseId, "IX_Payments_CourseID");

            entity.HasIndex(e => e.StudentId, "IX_Payments_StudentId");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.AdmissionId).HasColumnName("AdmissionID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.PaymentMode).HasMaxLength(50);

            entity.HasOne(d => d.Admission).WithMany(p => p.Payments).HasForeignKey(d => d.AdmissionId);

            entity.HasOne(d => d.Course).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Student).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PaymentErrror>(entity =>
        {
            entity.ToTable("payment errror");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<SpookyRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpookyRe__3214EC07ACD146A1");

            entity.Property(e => e.Id).HasMaxLength(64);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(30);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.StudentEmailId, "IX_Students_StudentEmailId").IsUnique();

            entity.HasIndex(e => e.UserId, "IX_Students_UserId").IsUnique();

            entity.Property(e => e.StudentEmailId).HasMaxLength(256);
            entity.Property(e => e.StudentName).HasMaxLength(150);

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Courses).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentCourse",
                    r => r.HasOne<Course>().WithMany().HasForeignKey("CourseId"),
                    l => l.HasOne<Student>().WithMany().HasForeignKey("StudentId"),
                    j =>
                    {
                        j.HasKey("StudentId", "CourseId");
                        j.ToTable("StudentCourses");
                        j.HasIndex(new[] { "CourseId" }, "IX_StudentCourses_CourseID");
                        j.IndexerProperty<int>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.UserRole).HasMaxLength(30);
            entity.Property(e => e.Username).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
