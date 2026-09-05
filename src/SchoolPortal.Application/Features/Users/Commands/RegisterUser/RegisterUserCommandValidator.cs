using FluentValidation;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Domain.Common;

namespace SchoolPortal.Application.Features.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IUserRepository users)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Must(p => p.Any(char.IsLetter) && p.Any(char.IsDigit))
            .WithMessage("Password must contain at least one letter and one digit.");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(RoleNames.All.Contains)
            .WithMessage($"Role must be one of: {string.Join(", ", RoleNames.All)}.");

        RuleFor(x => x.PhoneNumber).MaximumLength(20);

        RuleFor(x => x.PreferredLanguage)
            .Must(l => l is null or "en" or "ur")
            .WithMessage("Preferred language must be 'en' or 'ur'.");

        // Email is unique platform-wide (USERS is global, not tenant-scoped).
        RuleFor(x => x.Email)
            .MustAsync(async (email, ct) =>
                !await users.EmailExistsAsync(email.Trim().ToUpperInvariant(), ct))
            .WithMessage("An account with this email already exists.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
