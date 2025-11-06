using FluentValidation;
using IdentityServer.Common.Models;
using IdentityServer.Common.Constants;

namespace IdentityServer.Application.Validators;

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.GrantType)
            .NotEmpty().WithMessage("Grant type is required")
            .Must(BeValidGrantType).WithMessage("Invalid grant type");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required");

        RuleFor(x => x.ClientSecret)
            .NotEmpty().WithMessage("Client secret is required");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .When(x => x.GrantType == AuthConstants.GrantTypes.Password);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .When(x => x.GrantType == AuthConstants.GrantTypes.Password);

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required")
            .When(x => x.GrantType == AuthConstants.GrantTypes.RefreshToken);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Authorization code is required")
            .When(x => x.GrantType == AuthConstants.GrantTypes.AuthorizationCode);
    }

    private bool BeValidGrantType(string grantType)
    {
        var validGrantTypes = new[]
        {
            AuthConstants.GrantTypes.AuthorizationCode,
            AuthConstants.GrantTypes.ClientCredentials,
            AuthConstants.GrantTypes.Password,
            AuthConstants.GrantTypes.RefreshToken,
            AuthConstants.GrantTypes.Implicit
        };

        return validGrantTypes.Contains(grantType);
    }
}