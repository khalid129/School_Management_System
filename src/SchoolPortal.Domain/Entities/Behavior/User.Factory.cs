using SchoolPortal.Domain.Common;

namespace SchoolPortal.Domain.Entities;

// Guarded creation for the scaffolded User entity. USERS is a global (non-tenant-scoped,
// non-IAuditable) table, so nothing stamps it automatically — the factory sets every field.
public partial class User
{
    /// <summary>
    /// Creates a valid, not-yet-persisted user. The caller passes an already-hashed password
    /// (hashing is an Infrastructure concern). Email confirmation and school membership are
    /// handled by the caller.
    /// </summary>
    public static User Create(
        string email,
        string fullName,
        string? preferredLanguage,
        string passwordHash,
        string? phoneNumber,
        DateTime utcNow)
    {
        email = (email ?? string.Empty).Trim();
        fullName = (fullName ?? string.Empty).Trim();

        if (email.Length == 0 || !email.Contains('@'))
            throw new DomainException("A valid email is required.");
        if (fullName.Length == 0)
            throw new DomainException("Full name is required.");
        if (string.IsNullOrEmpty(passwordHash))
            throw new DomainException("A password hash is required.");

        var normalizedEmail = email.ToUpperInvariant();
        var language = preferredLanguage is "en" or "ur" ? preferredLanguage : "en";

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = normalizedEmail,
            UserName = email,
            NormalizedUserName = normalizedEmail,
            EmailConfirmed = false,
            PasswordHash = passwordHash,
            SecurityStamp = Guid.NewGuid().ToString("N"),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            FullName = fullName,
            PreferredLanguage = language,
            IsActive = true,
            CreatedOn = utcNow,
        };
    }
}
