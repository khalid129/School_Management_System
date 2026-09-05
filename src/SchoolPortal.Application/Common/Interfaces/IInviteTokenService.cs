namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>
/// Issues and validates the stateless invitation token handed back from user registration.
/// The token binds the user id to their current <c>SecurityStamp</c> via an HMAC, so it needs
/// no storage and is invalidated by rotating the stamp.
/// </summary>
public interface IInviteTokenService
{
    string Create(Guid userId, string securityStamp);

    /// <summary>Validates a token against the user's current stamp. For the future accept-invite flow.</summary>
    bool TryValidate(string token, string securityStamp, out Guid userId);
}
