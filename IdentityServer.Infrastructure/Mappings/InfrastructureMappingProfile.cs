using AutoMapper;
using IdentityServer.Domain.Entities;
using IdentityServer.Infrastructure.Persistence.Models;

namespace IdentityServer.Infrastructure.Mappings;

public class InfrastructureMappingProfile : Profile
{
    public InfrastructureMappingProfile()
    {
        // Client mappings
        CreateMap<Client, ClientModel>().ReverseMap();

        // User mappings
        CreateMap<User, UserModel>().ReverseMap()
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId));

        // Scope mappings
        CreateMap<Scope, ScopeModel>().ReverseMap();

        // GrantType mappings
        CreateMap<GrantType, GrantTypeModel>().ReverseMap();

        // UserClientScope mappings
        CreateMap<UserClientScope, UserClientScopeModel>().ReverseMap();

        // RefreshToken mappings
        CreateMap<RefreshToken, RefreshTokenModel>().ReverseMap();

        // AuthorizationCode mappings
        CreateMap<AuthorizationCode, AuthorizationCodeModel>().ReverseMap();

        // AppSetting mappings
        CreateMap<AppSetting, AppSettingModel>().ReverseMap();

        // ExceptionLog mappings
        CreateMap<ExceptionLog, ExceptionLogModel>().ReverseMap();

        // RequestResponseLog mappings
        CreateMap<RequestResponseLog, RequestResponseLogModel>().ReverseMap();
    }
}