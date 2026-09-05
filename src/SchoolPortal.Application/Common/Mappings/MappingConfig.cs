using Mapster;
using SchoolPortal.Domain.Entities;
using SchoolPortal.Application.Features.Students.Queries.GetAllStudents;
using SchoolPortal.Application.Features.Students.Queries.GetStudentById;

namespace SchoolPortal.Application.Common.Mappings;

/// <summary>
/// Mapster registrations, discovered by <c>TypeAdapterConfig.GlobalSettings.Scan(...)</c>
/// in <c>DependencyInjection</c>. All source/target member names line up 1:1 today, so
/// these are effectively documentation — but keeping them explicit means a rename breaks
/// the build here rather than silently dropping a field.
/// </summary>
public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Student, StudentDto>();
        config.NewConfig<Student, StudentListItemDto>();
    }
}
