using SchoolPortal.Domain.Common;

namespace SchoolPortal.Domain.Entities;

// Reference pattern for guarding a scaffolded entity. The scaffolder emits public setters
// (EF needs them), so this does not achieve hard encapsulation; instead it gives the
// Application layer one intentional, invariant-checked way to create and mutate a Student.
// Other entities follow this same shape as they gain behaviour.
public partial class Student
{
    /// <summary>
    /// Creates a valid, not-yet-persisted Student. SchoolId is intentionally left unset —
    /// the SaveChanges interceptor stamps it from the current tenant on insert.
    /// </summary>
    public static Student Create(
        string admissionNumber,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        DateOnly admissionDate,
        string? gender = null,
        Guid? currentClassSectionId = null,
        string? rollNumber = null)
    {
        admissionNumber = (admissionNumber ?? string.Empty).Trim();
        firstName = (firstName ?? string.Empty).Trim();
        lastName = (lastName ?? string.Empty).Trim();

        if (admissionNumber.Length == 0)
            throw new DomainException("Admission number is required.");
        if (firstName.Length == 0)
            throw new DomainException("First name is required.");
        if (lastName.Length == 0)
            throw new DomainException("Last name is required.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dateOfBirth >= today)
            throw new DomainException("Date of birth must be in the past.");
        if (admissionDate > today)
            throw new DomainException("Admission date cannot be in the future.");

        return new Student
        {
            Id = Guid.NewGuid(),
            AdmissionNumber = admissionNumber,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            AdmissionDate = admissionDate,
            Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim(),
            CurrentClassSectionId = currentClassSectionId,
            RollNumber = string.IsNullOrWhiteSpace(rollNumber) ? null : rollNumber.Trim(),
            Status = StudentStatus.Active,
            IsActive = true,
            IsDeleted = false,
        };
    }

    /// <summary>Updates the mutable profile fields, re-checking invariants.</summary>
    public void UpdateProfile(string firstName, string lastName, string? gender, string? rollNumber)
    {
        firstName = (firstName ?? string.Empty).Trim();
        lastName = (lastName ?? string.Empty).Trim();

        if (firstName.Length == 0)
            throw new DomainException("First name is required.");
        if (lastName.Length == 0)
            throw new DomainException("Last name is required.");

        FirstName = firstName;
        LastName = lastName;
        Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim();
        RollNumber = string.IsNullOrWhiteSpace(rollNumber) ? null : rollNumber.Trim();
    }
}

/// <summary>Business lifecycle states for a student (STUDENTS.STATUS), distinct from IsActive.</summary>
public static class StudentStatus
{
    public const string Active = "Active";
    public const string Graduated = "Graduated";
    public const string Withdrawn = "Withdrawn";
}
