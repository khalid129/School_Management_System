using Microsoft.AspNetCore.Identity;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Infrastructure.Identity;

/// <summary>
/// <see cref="IPasswordHasher"/> over ASP.NET Core Identity's <see cref="PasswordHasher{TUser}"/>
/// (PBKDF2, versioned hash format). The generic <c>TUser</c> argument is unused by the default
/// implementation, so a placeholder type is fine.
/// </summary>
public sealed class PasswordHasherAdapter : IPasswordHasher
{
    private static readonly PasswordHasher<object> Hasher = new();
    private static readonly object Placeholder = new();

    public string Hash(string password) => Hasher.HashPassword(Placeholder, password);

    public bool Verify(string hash, string password)
        => Hasher.VerifyHashedPassword(Placeholder, hash, password) != PasswordVerificationResult.Failed;
}
