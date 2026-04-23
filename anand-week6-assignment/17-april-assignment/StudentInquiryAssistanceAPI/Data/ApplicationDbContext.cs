using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enquiry> Enquiries => Set<Enquiry>();
    public DbSet<Admission> Admissions => Set<Admission>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentEmailId)
            .IsUnique();

        modelBuilder.Entity<Admission>()
            .HasIndex(a => new { a.StudentId, a.CourseId })
            .IsUnique();

        modelBuilder.Entity<Course>()
            .Property(c => c.FeesAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<User>()
            .HasOne(u => u.Student)
            .WithOne(s => s.User)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.CreatedByUser)
            .WithMany(u => u.Courses)
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Enquiry>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enquiries)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enquiry>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enquiries)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enquiry>()
            .HasOne(e => e.RespondedByUser)
            .WithMany(u => u.RepliedEnquiries)
            .HasForeignKey(e => e.RespondedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Admission>()
            .HasOne(a => a.Student)
            .WithMany(s => s.Admissions)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Admission>()
            .HasOne(a => a.Course)
            .WithMany(c => c.Admissions)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Student)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Admission)
            .WithMany(a => a.Payments)
            .HasForeignKey(p => p.AdmissionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Course)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
