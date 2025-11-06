using FluentValidation;
using IdentityServer.Application.DTOs;

namespace IdentityServer.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 255).WithMessage("Username must be between 3 and 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

        RuleFor(x => x.EmailAddress)
            .EmailAddress().WithMessage("Invalid email address format")
            .When(x => !string.IsNullOrEmpty(x.EmailAddress));

        RuleFor(x => x.Firstname)
            .MaximumLength(255).WithMessage("First name cannot exceed 255 characters");

        RuleFor(x => x.Lastname)
            .MaximumLength(255).WithMessage("Last name cannot exceed 255 characters");
    }
}