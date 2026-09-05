namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>
/// One-way password hashing. Implemented in Infrastructure over
/// <c>Microsoft.AspNetCore.Identity.PasswordHasher</c> so the Application layer stays
/// free of the Identity dependency.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string hash, string password);
}
