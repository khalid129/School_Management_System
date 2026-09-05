using FluentAssertions;
using SchoolPortal.Domain.Common;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Domain.Tests;

public class StudentFactoryTests
{
    private static readonly DateOnly Dob = new(2015, 1, 1);
    private static readonly DateOnly Admission = new(2026, 1, 1);

    [Fact]
    public void Create_with_valid_input_returns_active_student_without_tenant()
    {
        var student = Student.Create("ADM-1", "Ayesha", "Khan", Dob, Admission, "Female");

        student.Id.Should().NotBeEmpty();
        student.FirstName.Should().Be("Ayesha");
        student.Status.Should().Be(StudentStatus.Active);
        student.IsActive.Should().BeTrue();
        student.IsDeleted.Should().BeFalse();
        student.SchoolId.Should().Be(Guid.Empty, "the SaveChanges interceptor stamps the tenant, not the factory");
    }

    [Theory]
    [InlineData("", "Khan")]
    [InlineData("Ayesha", "")]
    [InlineData("   ", "Khan")]
    public void Create_rejects_blank_names(string first, string last)
    {
        var act = () => Student.Create("ADM-1", first, last, Dob, Admission);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_rejects_future_date_of_birth()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
        var act = () => Student.Create("ADM-1", "Ayesha", "Khan", future, Admission);
        act.Should().Throw<DomainException>().WithMessage("*past*");
    }

    [Fact]
    public void Create_rejects_empty_admission_number()
    {
        var act = () => Student.Create("  ", "Ayesha", "Khan", Dob, Admission);
        act.Should().Throw<DomainException>().WithMessage("*Admission number*");
    }

    [Fact]
    public void UpdateProfile_rejects_blank_name()
    {
        var student = Student.Create("ADM-1", "Ayesha", "Khan", Dob, Admission);
        var act = () => student.UpdateProfile("", "Khan", null, null);
        act.Should().Throw<DomainException>();
    }
}
