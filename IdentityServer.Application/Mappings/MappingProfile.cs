using AutoMapper;
using IdentityServer.Application.DTOs;
using IdentityServer.Domain.Entities;

namespace IdentityServer.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.GrantTypeName, opt => opt.MapFrom(src => src.GrantType != null ? src.GrantType.Name : string.Empty));

        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();

        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();

        CreateMap<Scope, ScopeDto>();
        CreateMap<GrantType, GrantTypeDto>();
        CreateMap<RefreshToken, RefreshTokenDto>();
    }
}

public class ScopeDto
{
    public int ScopeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class GrantTypeDto
{
    public int GrantTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
}

public class RefreshTokenDto
{
    public int RefreshTokenId { get; set; }
    public string? Token { get; set; }
    public int? UserId { get; set; }
    public int? OAuthClientId { get; set; }
    public DateTime? ExpiresOn { get; set; }
    public bool? IsRevoked { get; set; }
}