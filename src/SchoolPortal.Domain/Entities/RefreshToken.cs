using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? CreatedByIp { get; set; }

    public DateTime? RevokedOn { get; set; }

    public string? RevokedByIp { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public virtual ICollection<RefreshToken> InverseReplacedByToken { get; set; } = new List<RefreshToken>();

    public virtual RefreshToken? ReplacedByToken { get; set; }

    public virtual User User { get; set; } = null!;
}
