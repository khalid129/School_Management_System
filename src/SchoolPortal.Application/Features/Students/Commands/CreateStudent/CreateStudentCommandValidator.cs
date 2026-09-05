using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.AdmissionNumber)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).MaximumLength(10);
        RuleFor(x => x.RollNumber).MaximumLength(10);

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob < DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.AdmissionDate)
            .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Admission date cannot be in the future.");

        // Uniqueness is per-tenant: the Students set is already tenant-filtered, so this
        // check only sees the current school's rows.
        RuleFor(x => x.AdmissionNumber)
            .MustAsync(async (admissionNumber, ct) =>
                !await db.Students.AnyAsync(s => s.AdmissionNumber == admissionNumber, ct))
            .WithMessage("A student with this admission number already exists.")
            .When(x => !string.IsNullOrWhiteSpace(x.AdmissionNumber));
    }
}
