namespace SchoolPortal.Domain.Common.Interfaces;

/// <summary>
/// Entities carrying the standard audit columns. The SaveChanges interceptor sets
/// <see cref="CreatedOn"/>/<see cref="CreatedBy"/> on insert and
/// <see cref="UpdatedOn"/>/<see cref="UpdatedBy"/> on update.
/// Members match the scaffolded columns CREATED_ON / CREATED_BY / UPDATED_ON / UPDATED_BY.
/// </summary>
public interface IAuditable
{
    DateTime CreatedOn { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedOn { get; set; }
    Guid? UpdatedBy { get; set; }
}
