using System.Security.Claims;
using System.Security.Principal;

namespace ClotheStore.Extension
{
    public static class IdentityExtension
    {
        public static string GetAccountId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AccountId");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetRoleId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("RoleId");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUsername(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Username");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetAvatar(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Avatar");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal,string claimType)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
