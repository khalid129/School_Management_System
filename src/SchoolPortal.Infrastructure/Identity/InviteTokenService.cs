using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Infrastructure.Identity;

/// <summary>
/// Stateless invitation token: <c>base64url( userId(16 bytes) || HMACSHA256("{userId}:{stamp}", key) )</c>.
/// No storage; invalidated when the user's <c>SecurityStamp</c> rotates. Key from
/// <c>Security:InviteTokenKey</c> (base64) — must come from Key Vault in production.
/// </summary>
public sealed class InviteTokenService : IInviteTokenService
{
    public const string KeyConfigPath = "Security:InviteTokenKey";
    private const int MacLength = 32;

    private readonly byte[] _key;

    public InviteTokenService(IConfiguration configuration)
    {
        var configured = configuration[KeyConfigPath];
        if (string.IsNullOrWhiteSpace(configured))
            throw new InvalidOperationException(
                $"Configuration '{KeyConfigPath}' is required for invite tokens.");

        _key = Convert.FromBase64String(configured);
    }

    public string Create(Guid userId, string securityStamp)
    {
        var mac = ComputeMac(userId, securityStamp);
        var payload = new byte[16 + MacLength];
        userId.TryWriteBytes(payload);
        mac.CopyTo(payload, 16);
        return Base64UrlEncode(payload);
    }

    public bool TryValidate(string token, string securityStamp, out Guid userId)
    {
        userId = Guid.Empty;
        if (string.IsNullOrWhiteSpace(token))
            return false;

        byte[] payload;
        try
        {
            payload = Base64UrlDecode(token);
        }
        catch (FormatException)
        {
            return false;
        }

        if (payload.Length != 16 + MacLength)
            return false;

        var candidateId = new Guid(payload.AsSpan(0, 16));
        var expected = ComputeMac(candidateId, securityStamp);

        if (!CryptographicOperations.FixedTimeEquals(payload.AsSpan(16), expected))
            return false;

        userId = candidateId;
        return true;
    }

    private byte[] ComputeMac(Guid userId, string securityStamp)
    {
        var data = Encoding.UTF8.GetBytes($"{userId:N}:{securityStamp}");
        return HMACSHA256.HashData(_key, data);
    }

    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] Base64UrlDecode(string value)
    {
        var s = value.Replace('-', '+').Replace('_', '/');
        s = (s.Length % 4) switch
        {
            2 => s + "==",
            3 => s + "=",
            0 => s,
            _ => throw new FormatException("Invalid base64url string."),
        };
        return Convert.FromBase64String(s);
    }
}
