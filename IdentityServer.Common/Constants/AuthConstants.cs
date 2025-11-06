using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Common.Constants
{
    public static class AuthConstants
    {
        public static class GrantTypes
        {
            public const string AuthorizationCode = "authorization_code";
            public const string ClientCredentials = "client_credentials";
            public const string Password = "password";
            public const string RefreshToken = "refresh_token";
            public const string Implicit = "implicit";
        }

        public static class Scopes
        {
            public const string Read = "read";
            public const string Write = "write";
            public const string Delete = "delete";
            public const string Admin = "admin";
            public const string Profile = "profile";
            public const string Email = "email";
            public const string OpenId = "openid";
            public const string OfflineAccess = "offline_access";
        }

        public static class ClaimTypes
        {
            public const string UserId = "user_id";
            public const string ClientId = "client_id";
            public const string Scope = "scope";
            public const string Sub = "sub";
            public const string Aud = "aud";
            public const string Iss = "iss";
            public const string Exp = "exp";
            public const string Iat = "iat";
        }
    }
}
