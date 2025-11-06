using FluentValidation;
using IdentityServer.Application.DTOs;

namespace IdentityServer.Application.Validators;

public class CreateClientValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required")
            .Length(3, 300).WithMessage("Client ID must be between 3 and 300 characters");

        RuleFor(x => x.ClientSecret)
            .NotEmpty().WithMessage("Client Secret is required")
            .MinimumLength(8).WithMessage("Client Secret must be at least 8 characters");

        RuleFor(x => x.Name)
            .MaximumLength(250).WithMessage("Name cannot exceed 250 characters");

        RuleFor(x => x.AccessTokenValidity)
            .GreaterThan(0).WithMessage("Access Token Validity must be greater than 0")
            .When(x => x.AccessTokenValidity.HasValue);
    }
}