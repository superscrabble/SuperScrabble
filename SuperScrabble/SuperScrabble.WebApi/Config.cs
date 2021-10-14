namespace SuperScrabble.WebApi
{
    using System.Collections.Generic;

    using IdentityServer4;
    using IdentityServer4.Models;

    public class Config
    {
        public const string WebApi = "WebApi";
        public const string Bearer = "Bearer";

        public static IEnumerable<IdentityResource> GetIdentityResources()
            => new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };

        public static IEnumerable<ApiResource> GetAPIs()
            => new List<ApiResource> { new ApiResource(WebApi, WebApi) { Scopes = { WebApi } } };

        public static IEnumerable<ApiScope> GetAPIScopes()
            => new List<ApiScope> { new ApiScope(WebApi, WebApi) };

        public static IEnumerable<Client> GetClients()
            => new List<Client>
            {
                new Client
                {
                    ClientId = "api",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        WebApi,
                    }
                }
            };
    }
}
