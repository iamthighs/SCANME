using QRCodeAttendance.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace QRCodeAttendance.Helpers
{
    public class AppIdentityUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppIdentityUser>
    {
        public AppIdentityUserClaimsPrincipalFactory(UserManager<AppIdentityUser> userManager, IOptions<IdentityOptions> options)
            : base(userManager, options)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppIdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("UserEmail", user.UserName ?? ""));
            identity.AddClaim(new Claim("UserFirstName", user.FirstName ?? ""));
            identity.AddClaim(new Claim("UserLastName", user.LastName ?? ""));
            identity.AddClaim(new Claim("UserProfile", user.ImageUrl ?? ""));
            return identity;
        }

        








    }
}
