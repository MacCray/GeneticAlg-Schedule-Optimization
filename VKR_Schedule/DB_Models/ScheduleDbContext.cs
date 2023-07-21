using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VKR_Schedule.DB_Models;

public partial class ScheduleDbContext : DbContext
{
    public ScheduleDbContext()
    {
    }

    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options)
        : base(options)
    {
        //this.Database.SetCommandTimeout(TimeSpan.FromSeconds(90));
    }

    public virtual DbSet<Auditoria> Auditorias { get; set; }

    public virtual DbSet<DaysOfWeek> DaysOfWeeks { get; set; }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Time> Times { get; set; }

    public virtual DbSet<TypesOfWeek> TypesOfWeeks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ScheduleDB;Trusted_Connection=True;TrustServerCertificate=True", options => { options.CommandTimeout(120); });

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(e => e.AuditoriaId).HasName("PK__tbAudito__D7259D520453B3E9");

            entity.Property(e => e.AuditoriaBuilding).HasMaxLength(100);
            entity.Property(e => e.AuditoriaName).HasMaxLength(20);
        });

        modelBuilder.Entity<DaysOfWeek>(entity =>
        {
            entity.HasKey(e => e.DayOfWeekId).HasName("PK__tbDaysOf__84910FEF88DA4E13");

            entity.ToTable("DaysOfWeek");

            entity.Property(e => e.DayOfWeekId).ValueGeneratedNever();
            entity.Property(e => e.DayOfWeekName).HasMaxLength(20);
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.DisciplineId).HasName("PK__tbDiscip__5ECD4D4805C9B73A");

            entity.Property(e => e.DisciplineName).HasMaxLength(200);
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__Facultie__306F636EC89D4FAF");

            entity.Property(e => e.FacultyId).ValueGeneratedNever();
            entity.Property(e => e.FacultyName).HasMaxLength(100);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.GroupNumber)
                .HasMaxLength(15)
                .IsFixedLength()
                .HasColumnName("Group_number");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Groups)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_Faculties");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.LecturerId).HasName("PK__tbLectur__088FAACC171BB7C1");

            entity.Property(e => e.LecturerName).HasMaxLength(100);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.SсheduleId).HasName("PK_tbShedule");

            entity.HasOne(d => d.Auditoria).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AuditoriaId)
                .HasConstraintName("FK_tbShedule_tbAuditorias");

            entity.HasOne(d => d.DayOfWeek).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.DayOfWeekId)
                .HasConstraintName("FK_tbShedule_tbDaysOfWeek");

            entity.HasOne(d => d.Discipline).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.DisciplineId)
                .HasConstraintName("FK_tbShedule_tbDisciplines");

            entity.HasOne(d => d.Group).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbShedule_tbGroups");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK_tbShedule_tbLecturers");

            entity.HasOne(d => d.Time).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TimeId)
                .HasConstraintName("FK_Schedules_Times");

            entity.HasOne(d => d.TypeOfWeek).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TypeOfWeekId)
                .HasConstraintName("FK_tbShedule_tbTypeOfWeek");
        });

        modelBuilder.Entity<Time>(entity =>
        {
            entity.Property(e => e.TimeId).ValueGeneratedNever();
            entity.Property(e => e.TimePeriod).HasMaxLength(20);
        });

        modelBuilder.Entity<TypesOfWeek>(entity =>
        {
            entity.HasKey(e => e.TypeOfWeekId).HasName("PK_tbTypeOfWeek");

            entity.ToTable("TypesOfWeek");

            entity.Property(e => e.TypeName).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
