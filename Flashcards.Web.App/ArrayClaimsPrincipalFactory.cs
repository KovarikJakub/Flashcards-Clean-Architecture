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
            Console.WriteLine("DEBUG: Start ArrayClaimsPrincipalFactory.CreateUserAsync");

            try 
            {
                var user = await base.CreateUserAsync(account, options);
                var identity = (ClaimsIdentity)user.Identity;

                if (account != null)
                {
                    Console.WriteLine("DEBUG: Raw from token (account.AdditionalProperties):");
                    foreach (var prop in account.AdditionalProperties)
                    {
                        Console.WriteLine($" - key: '{prop.Key}', value: '{prop.Value}'");
                    }

                    foreach (var kvp in account.AdditionalProperties)
                    {
                        if (kvp.Key == "role" && kvp.Value != null)
                        {
                            Console.WriteLine("DEBUG: found  'role'.");

                            if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                            {
                                Console.WriteLine("DEBUG: Value is JSON Array (pole)....");
                                
                                var existingClaim = identity.FindFirst("role");
                                if (existingClaim != null)
                                {
                                    Console.WriteLine($"DEBUG: removing old claim: {existingClaim.Value}");
                                    identity.RemoveClaim(existingClaim);
                                }
                                else
                                {
                                    Console.WriteLine("DEBUG: old claim not found, okay?.");
                                }
                                
                                foreach (var item in element.EnumerateArray())
                                {
                                    var roleValue = item.ToString();
                                    Console.WriteLine($"DEBUG: Adding  role: '{roleValue}'");
                                    identity.AddClaim(new Claim("role", roleValue));
                                }
                            }
                            else
                            {
                                Console.WriteLine($"DEBUG: Value role is not array but:: {kvp.Value.GetType().Name} (Value: {kvp.Value})");
                            }
                        }
                    }
                }
                
                Console.WriteLine($"DEBUG: Finished. user has {identity.Claims.Count()} claims.");
                var adminRole = identity.Claims.FirstOrDefault(c => c.Type == "role" && c.Value == "Admin");
                Console.WriteLine($"DEBUG: Has admin role? {(adminRole != null ? "Yes" : "No")}");

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ArrayClaimsPrincipalFactory: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; 
            }
        }
    }
}