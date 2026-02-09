using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PortalActividades.Data.Models;

namespace PortalActividades.Data.Contexts;

public partial class PortalActividadesDbContext : DbContext
{
    public PortalActividadesDbContext()
    {
    }

    public PortalActividadesDbContext(DbContextOptions<PortalActividadesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Career> Careers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Organizer> Organizers { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=PortalActividadesDB;User ID=derek;Password=Derek@2026_Sql;Trusted_Connection=False;MultipleActiveResultSets=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activiti__3214EC0747E10D80");

            entity.HasIndex(e => e.CategoryId, "IX_Activities_Category");

            entity.HasIndex(e => e.Date, "IX_Activities_Date");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Activities)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Activitie__Categ__4D94879B");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Activities)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Activitie__Organ__4E88ABD4");
        });

        modelBuilder.Entity<Career>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Careers__3214EC0766BD612E");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Faculty).WithMany(p => p.Careers)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Careers__Faculty__3C69FB99");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0763BF866C");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Enrollme__3214EC077B11AFB0");

            entity.HasIndex(e => e.ActivityId, "IX_Enrollments_Activity");

            entity.HasIndex(e => e.Status, "IX_Enrollments_Status");

            entity.HasIndex(e => e.StudentId, "IX_Enrollments_Student");

            entity.HasIndex(e => new { e.ActivityId, e.StudentId }, "UQ__Enrollme__C6D8F529B2FB84B1").IsUnique();

            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Activity).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Activ__5441852A");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Stude__5535A963");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Facultie__3214EC0727BBD3B1");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Organize__1788CC4CE9ABC5BB");

            entity.HasIndex(e => e.Department, "IX_Organizers_Department");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.Shifts).HasMaxLength(200);
            entity.Property(e => e.WorkDays).HasMaxLength(200);

            entity.HasOne(d => d.User).WithOne(p => p.Organizer)
                .HasForeignKey<Organizer>(d => d.UserId)
                .HasConstraintName("FK__Organizer__UserI__440B1D61");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ratings__3214EC073F41A86A");

            entity.HasIndex(e => e.ActivityId, "IX_Ratings_Activity");

            entity.Property(e => e.RatingDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Activity).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ratings__Activit__59FA5E80");

            entity.HasOne(d => d.Student).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ratings__Student__5AEE82B9");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Students__1788CC4C35CDF610");

            entity.HasIndex(e => e.CareerId, "IX_Students_Career");

            entity.HasIndex(e => e.Modality, "IX_Students_Modality");

            entity.HasIndex(e => e.Schedule, "IX_Students_Schedule");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Modality).HasMaxLength(20);
            entity.Property(e => e.Schedule).HasMaxLength(20);

            entity.HasOne(d => d.Career).WithMany(p => p.Students)
                .HasForeignKey(d => d.CareerId)
                .HasConstraintName("FK__Students__Career__49C3F6B7");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .HasConstraintName("FK__Students__UserId__48CFD27E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC074187461E");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.Role, "IX_Users_Role");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053415E2E1A0").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
