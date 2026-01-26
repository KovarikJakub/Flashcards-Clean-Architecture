using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace Flashcards.Web.App
{
    public class ArrayClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public ArrayClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor)
            : base(accessor)
        {
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            var identity = (ClaimsIdentity)user.Identity;

            if (account != null)
            {
                foreach (var kvp in account.AdditionalProperties)
                {
                    if (kvp.Key == "role" && kvp.Value != null)
                    {
                        if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                        {
                            identity.RemoveClaim(identity.FindFirst("role"));
                            
                            foreach (var item in element.EnumerateArray())
                            {
                                identity.AddClaim(new Claim("role", item.ToString()));
                            }
                        }
                    }
                }
            }

            return user;
        }
    }
}