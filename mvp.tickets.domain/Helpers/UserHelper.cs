using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Models;
using System.Security.Claims;

namespace mvp.tickets.domain.Helpers
{
    public static class UserHelper
    {
        public static List<Claim> GetClaims(IUserModel userModel, bool isRoot)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, userModel.Id.ToString()),
                    new Claim(AuthConstants.CompanyIdClaim, userModel.CompanyId.ToString()),
                    new Claim(AuthConstants.UserDataClaim, System.Text.Json.JsonSerializer.Serialize(userModel)),
                };

            if (userModel.Permissions.HasFlag(Permissions.Admin))
            {
                claims.Add(new Claim(AuthConstants.AdminClaim, "true"));
                if (isRoot)
                {
                    claims.Add(new Claim(AuthConstants.RootSpaceClaim, "true"));
                }
            }
            if (userModel.Permissions.HasFlag(Permissions.Employee))
            {
                claims.Add(new Claim(AuthConstants.EmployeeClaim, "true"));
            }
            if (userModel.Permissions.HasFlag(Permissions.User))
            {
                claims.Add(new Claim(AuthConstants.UserClaim, "true"));
            }
            return claims;
        }
    }
}
