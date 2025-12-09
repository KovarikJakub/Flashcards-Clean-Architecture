using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Flashcards.IdentityProvider.App
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                var profileIdentityResources = new IdentityResources.Profile();
                profileIdentityResources.UserClaims.Add("username");

                return
                [
                    new IdentityResources.OpenId(),
                    profileIdentityResources
                ];
            }
        }

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new ("flashcardsclientaudience")
        ];

        public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ("flashcardsapi", [JwtClaimTypes.Role])
        ];

        public static IEnumerable<Client> Clients =>
        [
            new()
            {
                ClientName = "Flashcards Client",
                ClientId = "flashcardsclient",
                AllowOfflineAccess = true,
                RedirectUris =
                [
                    "https://oauth.pstmn.io/v1/callback",
                    "https://localhost:44355/authentication/login-callback",
                ],
                PostLogoutRedirectUris =
                [
                    "https://localhost:44355"
                ],
                AllowedGrantTypes =
                [
                    GrantType.ClientCredentials,
                    GrantType.ResourceOwnerPassword,
                    GrantType.AuthorizationCode
                ],
                RequirePkce = true,
                AllowedScopes =
                [
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "flashcardsapi"
                ],
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                RequireClientSecret = false
            }
        ];
    }
}
