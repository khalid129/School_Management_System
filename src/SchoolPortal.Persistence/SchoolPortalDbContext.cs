using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Persistence;

public partial class SchoolPortalDbContext : DbContext
{
    public SchoolPortalDbContext(DbContextOptions<SchoolPortalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicYear> AcademicYears { get; set; }

    public virtual DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    public virtual DbSet<ClassLevel> ClassLevels { get; set; }

    public virtual DbSet<ClassSection> ClassSections { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamResult> ExamResults { get; set; }

    public virtual DbSet<GuardianRelationship> GuardianRelationships { get; set; }

    public virtual DbSet<PlatformAdministrator> PlatformAdministrators { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<SubscriptionPlanFeature> SubscriptionPlanFeatures { get; set; }

    public virtual DbSet<TimetableSlot> TimetableSlots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSchoolMembership> UserSchoolMemberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicYear>(entity =>
        {
            entity.ToTable("ACADEMIC_YEARS");

            entity.HasIndex(e => new { e.SchoolId, e.IsCurrent }, "IX_ACADEMIC_YEARS_SCHOOL_IS_CURRENT");

            entity.HasIndex(e => new { e.SchoolId, e.Name }, "UQ_ACADEMIC_YEARS_SCHOOL_NAME").IsUnique();

            entity.HasIndex(e => e.SchoolId, "UX_ACADEMIC_YEARS_ONE_CURRENT_PER_SCHOOL")
                .IsUnique()
                .HasFilter("([IS_CURRENT]=(1))");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.EndDate).HasColumnName("END_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsCurrent).HasColumnName("IS_CURRENT");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("NAME");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.StartDate).HasColumnName("START_DATE");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AcademicYearCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_ACADEMIC_YEARS_USERS_CREATEDBY");

            // Manual correction: the scaffolder mis-inferred School<->AcademicYear as 1:1
            // because of the filtered unique index UX_ACADEMIC_YEARS_ONE_CURRENT_PER_SCHOOL
            // (unique on SCHOOL_ID alone, WHERE IS_CURRENT = 1). It is really 1:many.
            // tools/rescaffold.ps1 re-applies this after every re-scaffold.
            entity.HasOne(d => d.School).WithMany(p => p.AcademicYears)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACADEMIC_YEARS_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AcademicYearUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_ACADEMIC_YEARS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("ATTENDANCE_RECORDS");

            entity.HasIndex(e => new { e.SchoolId, e.AttendanceDate }, "CIX_ATTENDANCE_RECORDS_SCHOOL_DATE").IsClustered();

            entity.HasIndex(e => new { e.SchoolId, e.ClassSectionId, e.AttendanceDate }, "IX_ATTENDANCE_RECORDS_SCHOOL_SECTION_DATE");

            entity.HasIndex(e => new { e.SchoolId, e.StudentId, e.AttendanceDate }, "UQ_ATTENDANCE_RECORDS_SCHOOL_STUDENT_DATE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AttendanceDate).HasColumnName("ATTENDANCE_DATE");
            entity.Property(e => e.ClassSectionId).HasColumnName("CLASS_SECTION_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.MarkedByStaffId).HasColumnName("MARKED_BY_STAFF_ID");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("STATUS");
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ATTENDANCE_RECORDS_CLASS_SECTIONS");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AttendanceRecordCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_ATTENDANCE_RECORDS_USERS_CREATEDBY");

            entity.HasOne(d => d.MarkedByStaff).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.MarkedByStaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ATTENDANCE_RECORDS_STAFF");

            entity.HasOne(d => d.School).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ATTENDANCE_RECORDS_SCHOOLS");

            entity.HasOne(d => d.Student).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ATTENDANCE_RECORDS_STUDENTS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AttendanceRecordUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_ATTENDANCE_RECORDS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<ClassLevel>(entity =>
        {
            entity.ToTable("CLASS_LEVELS");

            entity.HasIndex(e => new { e.SchoolId, e.SortOrder }, "IX_CLASS_LEVELS_SCHOOL_SORT_ORDER");

            entity.HasIndex(e => new { e.SchoolId, e.Name }, "UQ_CLASS_LEVELS_SCHOOL_NAME").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.SortOrder).HasColumnName("SORT_ORDER");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ClassLevelCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_CLASS_LEVELS_USERS_CREATEDBY");

            entity.HasOne(d => d.School).WithMany(p => p.ClassLevels)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLASS_LEVELS_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ClassLevelUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_CLASS_LEVELS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<ClassSection>(entity =>
        {
            entity.ToTable("CLASS_SECTIONS");

            entity.HasIndex(e => new { e.SchoolId, e.AcademicYearId }, "IX_CLASS_SECTIONS_SCHOOL_ACADEMIC_YEAR");

            entity.HasIndex(e => new { e.SchoolId, e.ClassLevelId, e.AcademicYearId, e.Name }, "UQ_CLASS_SECTIONS_SCHOOL_LEVEL_YEAR_NAME").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.AcademicYearId).HasColumnName("ACADEMIC_YEAR_ID");
            entity.Property(e => e.Capacity).HasColumnName("CAPACITY");
            entity.Property(e => e.ClassLevelId).HasColumnName("CLASS_LEVEL_ID");
            entity.Property(e => e.ClassTeacherStaffId).HasColumnName("CLASS_TEACHER_STAFF_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("NAME");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLASS_SECTIONS_ACADEMIC_YEARS");

            entity.HasOne(d => d.ClassLevel).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ClassLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLASS_SECTIONS_CLASS_LEVELS");

            entity.HasOne(d => d.ClassTeacherStaff).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.ClassTeacherStaffId)
                .HasConstraintName("FK_CLASS_SECTIONS_STAFF");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ClassSectionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_CLASS_SECTIONS_USERS_CREATEDBY");

            entity.HasOne(d => d.School).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLASS_SECTIONS_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ClassSectionUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_CLASS_SECTIONS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("ENROLLMENTS");

            entity.HasIndex(e => new { e.SchoolId, e.AcademicYearId }, "IX_ENROLLMENTS_SCHOOL_ACADEMIC_YEAR");

            entity.HasIndex(e => new { e.SchoolId, e.ClassSectionId }, "IX_ENROLLMENTS_SCHOOL_CLASS_SECTION");

            entity.HasIndex(e => new { e.StudentId, e.AcademicYearId }, "UQ_ENROLLMENTS_STUDENT_ACADEMIC_YEAR").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.AcademicYearId).HasColumnName("ACADEMIC_YEAR_ID");
            entity.Property(e => e.ClassSectionId).HasColumnName("CLASS_SECTION_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.EnrollmentDate).HasColumnName("ENROLLMENT_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("STATUS");
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ENROLLMENTS_ACADEMIC_YEARS");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ENROLLMENTS_CLASS_SECTIONS");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EnrollmentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_ENROLLMENTS_USERS_CREATEDBY");

            entity.HasOne(d => d.School).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ENROLLMENTS_SCHOOLS");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ENROLLMENTS_STUDENTS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EnrollmentUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_ENROLLMENTS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.ToTable("EXAMS");

            entity.HasIndex(e => new { e.SchoolId, e.AcademicYearId, e.ClassLevelId, e.Name }, "UQ_EXAMS_SCHOOL_YEAR_LEVEL_NAME").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.AcademicYearId).HasColumnName("ACADEMIC_YEAR_ID");
            entity.Property(e => e.ClassLevelId).HasColumnName("CLASS_LEVEL_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.ExamEndDate).HasColumnName("EXAM_END_DATE");
            entity.Property(e => e.ExamStartDate).HasColumnName("EXAM_START_DATE");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("STATUS");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.Exams)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXAMS_ACADEMIC_YEARS");

            entity.HasOne(d => d.ClassLevel).WithMany(p => p.Exams)
                .HasForeignKey(d => d.ClassLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXAMS_CLASS_LEVELS");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExamCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_EXAMS_USERS_CREATEDBY");

            entity.HasOne(d => d.School).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXAMS_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ExamUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_EXAMS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<ExamResult>(entity =>
        {
            entity.ToTable("EXAM_RESULTS");

            entity.HasIndex(e => new { e.SchoolId, e.StudentId }, "IX_EXAM_RESULTS_SCHOOL_STUDENT");

            entity.HasIndex(e => new { e.ExamId, e.StudentId, e.SubjectId }, "UQ_EXAM_RESULTS_EXAM_STUDENT_SUBJECT").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.EnteredByStaffId).HasColumnName("ENTERED_BY_STAFF_ID");
            entity.Property(e => e.ExamId).HasColumnName("EXAM_ID");
            entity.Property(e => e.Grade)
                .HasMaxLength(5)
                .HasColumnName("GRADE");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.MarksObtained)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("MARKS_OBTAINED");
            entity.Property(e => e.MaxMarks)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("MAX_MARKS");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
            entity.Property(e => e.SubjectId).HasColumnName("SUBJECT_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExamResultCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_EXAM_RESULTS_USERS_CREATEDBY");

            entity.HasOne(d => d.EnteredByStaff).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.EnteredByStaffId)
                .HasConstraintName("FK_EXAM_RESULTS_STAFF");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXAM_RESULTS_EXAMS");

            entity.HasOne(d => d.School).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXAM_RESULTS_SCHOOLS");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXAM_RESULTS_STUDENTS");

            entity.HasOne(d => d.Subject).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EXAM_RESULTS_SUBJECTS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ExamResultUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_EXAM_RESULTS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<GuardianRelationship>(entity =>
        {
            entity.ToTable("GUARDIAN_RELATIONSHIPS");

            entity.HasIndex(e => new { e.SchoolId, e.GuardianUserId }, "IX_GUARDIAN_RELATIONSHIPS_SCHOOL_GUARDIAN");

            entity.HasIndex(e => new { e.StudentId, e.GuardianUserId }, "UQ_GUARDIAN_RELATIONSHIPS_STUDENT_GUARDIAN").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.GuardianUserId).HasColumnName("GUARDIAN_USER_ID");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.IsPrimaryContact).HasColumnName("IS_PRIMARY_CONTACT");
            entity.Property(e => e.RelationshipType)
                .HasMaxLength(20)
                .HasColumnName("RELATIONSHIP_TYPE");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GuardianRelationshipCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_GUARDIAN_RELATIONSHIPS_USERS_CREATEDBY");

            entity.HasOne(d => d.GuardianUser).WithMany(p => p.GuardianRelationshipGuardianUsers)
                .HasForeignKey(d => d.GuardianUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GUARDIAN_RELATIONSHIPS_USERS_GUARDIAN");

            entity.HasOne(d => d.School).WithMany(p => p.GuardianRelationships)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GUARDIAN_RELATIONSHIPS_SCHOOLS");

            entity.HasOne(d => d.Student).WithMany(p => p.GuardianRelationships)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GUARDIAN_RELATIONSHIPS_STUDENTS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.GuardianRelationshipUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_GUARDIAN_RELATIONSHIPS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<PlatformAdministrator>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("PLATFORM_ADMINISTRATORS");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("USER_ID");
            entity.Property(e => e.GrantedBy).HasColumnName("GRANTED_BY");
            entity.Property(e => e.GrantedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("GRANTED_ON");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");

            entity.HasOne(d => d.GrantedByNavigation).WithMany(p => p.PlatformAdministratorGrantedByNavigations)
                .HasForeignKey(d => d.GrantedBy)
                .HasConstraintName("FK_PLATFORM_ADMINISTRATORS_USERS_GRANTEDBY");

            entity.HasOne(d => d.User).WithOne(p => p.PlatformAdministratorUser)
                .HasForeignKey<PlatformAdministrator>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PLATFORM_ADMINISTRATORS_USERS_USERID");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("REFRESH_TOKENS");

            entity.HasIndex(e => e.UserId, "IX_REFRESH_TOKENS_USER_ID");

            entity.HasIndex(e => e.TokenHash, "UX_REFRESH_TOKENS_TOKEN_HASH").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedByIp)
                .HasMaxLength(45)
                .HasColumnName("CREATED_BY_IP");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.ExpiresOn).HasColumnName("EXPIRES_ON");
            entity.Property(e => e.ReplacedByTokenId).HasColumnName("REPLACED_BY_TOKEN_ID");
            entity.Property(e => e.RevokedByIp)
                .HasMaxLength(45)
                .HasColumnName("REVOKED_BY_IP");
            entity.Property(e => e.RevokedOn).HasColumnName("REVOKED_ON");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(256)
                .HasColumnName("TOKEN_HASH");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.ReplacedByToken).WithMany(p => p.InverseReplacedByToken)
                .HasForeignKey(d => d.ReplacedByTokenId)
                .HasConstraintName("FK_REFRESH_TOKENS_REFRESH_TOKENS_REPLACEDBY");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REFRESH_TOKENS_USERS");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("ROLES");

            entity.HasIndex(e => e.NormalizedName, "UX_ROLES_NORMALIZED_NAME").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("NAME");
            entity.Property(e => e.NormalizedName)
                .HasMaxLength(256)
                .HasColumnName("NORMALIZED_NAME");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.ToTable("SCHOOLS");

            entity.HasIndex(e => e.Name, "IX_SCHOOLS_NAME");

            entity.HasIndex(e => e.Status, "IX_SCHOOLS_STATUS");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasDefaultValue("Karachi")
                .HasColumnName("CITY");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.CurriculumBoard)
                .HasMaxLength(100)
                .HasColumnName("CURRICULUM_BOARD");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.LegalName)
                .HasMaxLength(200)
                .HasColumnName("LEGAL_NAME");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(500)
                .HasColumnName("LOGO_URL");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("NAME");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("STATUS");
            entity.Property(e => e.SubscriptionPlanId).HasColumnName("SUBSCRIPTION_PLAN_ID");
            entity.Property(e => e.TrialEndsOn).HasColumnName("TRIAL_ENDS_ON");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SchoolCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_SCHOOLS_USERS_CREATEDBY");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.Schools)
                .HasForeignKey(d => d.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SCHOOLS_SUBSCRIPTION_PLANS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SchoolUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_SCHOOLS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("STAFF");

            entity.HasIndex(e => new { e.SchoolId, e.Status }, "IX_STAFF_SCHOOL_STATUS");

            entity.HasIndex(e => new { e.SchoolId, e.EmployeeCode }, "UQ_STAFF_SCHOOL_EMPLOYEE_CODE").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ApplicationUserId).HasColumnName("APPLICATION_USER_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.DateOfBirth).HasColumnName("DATE_OF_BIRTH");
            entity.Property(e => e.Designation)
                .HasMaxLength(50)
                .HasColumnName("DESIGNATION");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("EMAIL");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(30)
                .HasColumnName("EMPLOYEE_CODE");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("FIRST_NAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("GENDER");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.JoiningDate).HasColumnName("JOINING_DATE");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("LAST_NAME");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500)
                .HasColumnName("PHOTO_URL");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active")
                .HasColumnName("STATUS");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.StaffApplicationUsers)
                .HasForeignKey(d => d.ApplicationUserId)
                .HasConstraintName("FK_STAFF_USERS_APPLICATIONUSER");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StaffCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_STAFF_USERS_CREATEDBY");

            entity.HasOne(d => d.School).WithMany(p => p.Staff)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_STAFF_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StaffUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_STAFF_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("STUDENTS");

            entity.HasIndex(e => new { e.SchoolId, e.CurrentClassSectionId }, "IX_STUDENTS_SCHOOL_CURRENT_SECTION");

            entity.HasIndex(e => new { e.SchoolId, e.Status }, "IX_STUDENTS_SCHOOL_STATUS");

            entity.HasIndex(e => new { e.SchoolId, e.AdmissionNumber }, "UQ_STUDENTS_SCHOOL_ADMISSION_NUMBER").IsUnique();

            entity.HasIndex(e => new { e.SchoolId, e.CurrentClassSectionId, e.RollNumber }, "UX_STUDENTS_SCHOOL_SECTION_ROLL")
                .IsUnique()
                .HasFilter("([CURRENT_CLASS_SECTION_ID] IS NOT NULL AND [ROLL_NUMBER] IS NOT NULL)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.AdmissionDate).HasColumnName("ADMISSION_DATE");
            entity.Property(e => e.AdmissionNumber)
                .HasMaxLength(30)
                .HasColumnName("ADMISSION_NUMBER");
            entity.Property(e => e.ApplicationUserId).HasColumnName("APPLICATION_USER_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.CurrentClassSectionId).HasColumnName("CURRENT_CLASS_SECTION_ID");
            entity.Property(e => e.DateOfBirth).HasColumnName("DATE_OF_BIRTH");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("FIRST_NAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("GENDER");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("LAST_NAME");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500)
                .HasColumnName("PHOTO_URL");
            entity.Property(e => e.RollNumber)
                .HasMaxLength(10)
                .HasColumnName("ROLL_NUMBER");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active")
                .HasColumnName("STATUS");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.StudentApplicationUsers)
                .HasForeignKey(d => d.ApplicationUserId)
                .HasConstraintName("FK_STUDENTS_USERS_APPLICATIONUSER");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StudentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_STUDENTS_USERS_CREATEDBY");

            entity.HasOne(d => d.CurrentClassSection).WithMany(p => p.Students)
                .HasForeignKey(d => d.CurrentClassSectionId)
                .HasConstraintName("FK_STUDENTS_CLASS_SECTIONS");

            entity.HasOne(d => d.School).WithMany(p => p.Students)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_STUDENTS_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StudentUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_STUDENTS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("SUBJECTS");

            entity.HasIndex(e => new { e.SchoolId, e.Name }, "UQ_SUBJECTS_SCHOOL_NAME").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("CODE");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SubjectCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_SUBJECTS_USERS_CREATEDBY");

            entity.HasOne(d => d.School).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SUBJECTS_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SubjectUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_SUBJECTS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("SUBSCRIPTION_PLANS");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("PKR")
                .HasColumnName("CURRENCY");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.MonthlyPriceAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONTHLY_PRICE_AMOUNT");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
            entity.Property(e => e.StudentCountMax).HasColumnName("STUDENT_COUNT_MAX");
            entity.Property(e => e.StudentCountMin).HasColumnName("STUDENT_COUNT_MIN");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SubscriptionPlanCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_SUBSCRIPTION_PLANS_USERS_CREATEDBY");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SubscriptionPlanUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_SUBSCRIPTION_PLANS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<SubscriptionPlanFeature>(entity =>
        {
            entity.ToTable("SUBSCRIPTION_PLAN_FEATURES");

            entity.HasIndex(e => new { e.SubscriptionPlanId, e.FeatureKey }, "UQ_SUBSCRIPTION_PLAN_FEATURES_PLAN_KEY").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.FeatureKey)
                .HasMaxLength(50)
                .HasColumnName("FEATURE_KEY");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(true)
                .HasColumnName("IS_ENABLED");
            entity.Property(e => e.SubscriptionPlanId).HasColumnName("SUBSCRIPTION_PLAN_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SubscriptionPlanFeatureCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_SUBSCRIPTION_PLAN_FEATURES_USERS_CREATEDBY");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.SubscriptionPlanFeatures)
                .HasForeignKey(d => d.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SUBSCRIPTION_PLAN_FEATURES_SUBSCRIPTION_PLANS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SubscriptionPlanFeatureUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_SUBSCRIPTION_PLAN_FEATURES_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<TimetableSlot>(entity =>
        {
            entity.ToTable("TIMETABLE_SLOTS");

            entity.HasIndex(e => new { e.SchoolId, e.StaffId, e.DayOfWeek }, "IX_TIMETABLE_SLOTS_SCHOOL_STAFF_DAY");

            entity.HasIndex(e => new { e.SchoolId, e.ClassSectionId, e.DayOfWeek, e.StartTime }, "UQ_TIMETABLE_SLOTS_SECTION_DAY_START").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.ClassSectionId).HasColumnName("CLASS_SECTION_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.DayOfWeek).HasColumnName("DAY_OF_WEEK");
            entity.Property(e => e.EndTime).HasColumnName("END_TIME");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .HasColumnName("ROOM_NAME");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.StaffId).HasColumnName("STAFF_ID");
            entity.Property(e => e.StartTime).HasColumnName("START_TIME");
            entity.Property(e => e.SubjectId).HasColumnName("SUBJECT_ID");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.TimetableSlots)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIMETABLE_SLOTS_CLASS_SECTIONS");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TimetableSlotCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_TIMETABLE_SLOTS_USERS_CREATEDBY");

            entity.HasOne(d => d.School).WithMany(p => p.TimetableSlots)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIMETABLE_SLOTS_SCHOOLS");

            entity.HasOne(d => d.Staff).WithMany(p => p.TimetableSlots)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIMETABLE_SLOTS_STAFF");

            entity.HasOne(d => d.Subject).WithMany(p => p.TimetableSlots)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIMETABLE_SLOTS_SUBJECTS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TimetableSlotUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_TIMETABLE_SLOTS_USERS_UPDATEDBY");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USERS");

            entity.HasIndex(e => e.NormalizedEmail, "UX_USERS_NORMALIZED_EMAIL")
                .IsUnique()
                .HasFilter("([NORMALIZED_EMAIL] IS NOT NULL)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.AccessFailedCount).HasColumnName("ACCESS_FAILED_COUNT");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("CONCURRENCY_STAMP");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("EMAIL");
            entity.Property(e => e.EmailConfirmed).HasColumnName("EMAIL_CONFIRMED");
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.LockoutEnabled)
                .HasDefaultValue(true)
                .HasColumnName("LOCKOUT_ENABLED");
            entity.Property(e => e.LockoutEnd).HasColumnName("LOCKOUT_END");
            entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(256)
                .HasColumnName("NORMALIZED_EMAIL");
            entity.Property(e => e.NormalizedUserName)
                .HasMaxLength(256)
                .HasColumnName("NORMALIZED_USER_NAME");
            entity.Property(e => e.PasswordHash).HasColumnName("PASSWORD_HASH");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("PHONE_NUMBER_CONFIRMED");
            entity.Property(e => e.PreferredLanguage)
                .HasMaxLength(5)
                .HasDefaultValue("en")
                .HasColumnName("PREFERRED_LANGUAGE");
            entity.Property(e => e.SecurityStamp).HasColumnName("SECURITY_STAMP");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("TWO_FACTOR_ENABLED");
            entity.Property(e => e.UserName)
                .HasMaxLength(256)
                .HasColumnName("USER_NAME");
        });

        modelBuilder.Entity<UserSchoolMembership>(entity =>
        {
            entity.ToTable("USER_SCHOOL_MEMBERSHIPS");

            entity.HasIndex(e => new { e.SchoolId, e.RoleId }, "IX_USER_SCHOOL_MEMBERSHIPS_SCHOOL_ROLE");

            entity.HasIndex(e => e.UserId, "IX_USER_SCHOOL_MEMBERSHIPS_USER_ID");

            entity.HasIndex(e => new { e.UserId, e.SchoolId, e.RoleId }, "UQ_USER_SCHOOL_MEMBERSHIPS_USER_SCHOOL_ROLE").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.InvitedOn).HasColumnName("INVITED_ON");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED");
            entity.Property(e => e.IsPrimary).HasColumnName("IS_PRIMARY");
            entity.Property(e => e.JoinedOn).HasColumnName("JOINED_ON");
            entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");
            entity.Property(e => e.SchoolId).HasColumnName("SCHOOL_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("STATUS");
            entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn).HasColumnName("UPDATED_ON");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserSchoolMembershipCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_USER_SCHOOL_MEMBERSHIPS_USERS_CREATEDBY");

            entity.HasOne(d => d.Role).WithMany(p => p.UserSchoolMemberships)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USER_SCHOOL_MEMBERSHIPS_ROLES");

            entity.HasOne(d => d.School).WithMany(p => p.UserSchoolMemberships)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USER_SCHOOL_MEMBERSHIPS_SCHOOLS");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UserSchoolMembershipUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_USER_SCHOOL_MEMBERSHIPS_USERS_UPDATEDBY");

            entity.HasOne(d => d.User).WithMany(p => p.UserSchoolMembershipUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USER_SCHOOL_MEMBERSHIPS_USERS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
