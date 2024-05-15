using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models.Entities;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<WorkingTime> WorkingTimes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.PatientEmail).HasMaxLength(50);
            entity.Property(e => e.PatientFullname).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_Appointments_Doctors");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(e => e.AcademicTitle).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Fullname).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Speciality).HasMaxLength(200);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        modelBuilder.Entity<WorkingTime>(entity =>
        {
            entity.Property(e => e.Friday).HasMaxLength(50);
            entity.Property(e => e.Monday).HasMaxLength(50);
            entity.Property(e => e.Saturday).HasMaxLength(50);
            entity.Property(e => e.Sunday).HasMaxLength(50);
            entity.Property(e => e.Thursday).HasMaxLength(50);
            entity.Property(e => e.Tuesday).HasMaxLength(50);
            entity.Property(e => e.Wednesday).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.WorkingTimes)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_WorkingTime_Doctors");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
