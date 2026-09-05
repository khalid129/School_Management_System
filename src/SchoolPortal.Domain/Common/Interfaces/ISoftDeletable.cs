namespace SchoolPortal.Domain.Common.Interfaces;

/// <summary>
/// Entities that are never hard-deleted by the application layer (Master-Plan "soft
/// deletes everywhere" DR rule). The EF Core global query filter hides rows where
/// <see cref="IsDeleted"/> is true. Matches the scaffolded IS_DELETED column.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
