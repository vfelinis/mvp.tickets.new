using Microsoft.IdentityModel.Tokens;
using mvp.tickets.domain.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace mvp.tickets.domain.Helpers
{
    public static class TokenHelper
    {
        private static string Secret = "MEgCQQC4u3aB2VLtTEgG0gBE6ptHr3lRfmxxXR4Eruec+WIdMkZZk4so7ruIaGbZUfi5BhyLMbI3EUe7nvCJL+ulOAPRAgMBAAE=";

        public static string GenerateToken(UserJWTData userData, int expirationMinutes)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userData)),
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        private static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);

                if (securityToken.ValidTo.ToUniversalTime() < DateTime.UtcNow)
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public static UserJWTData ValidateToken(string token)
        {
            var identity = GetTokenData(token);
            if (identity == null)
                return null;

            Claim UserDataClaim = identity.FindFirst(ClaimTypes.UserData);

            UserJWTData userData;
            try
            {
                userData = JsonConvert.DeserializeObject<UserJWTData>(UserDataClaim.Value);
            }
            catch
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(userData?.Email))
            {
                return null;
            }

            return userData;
        }

        private static ClaimsIdentity GetTokenData(string token)
        {
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;

            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }

            return identity;
        }
    }
}
