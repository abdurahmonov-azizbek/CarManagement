// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using CarManagement.Api.Models.Users;
using CarManagement.Api.Services.Foundations.Authorizations.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarManagement.Api.Services.Foundations.Authorizations
{
    public class AccessTokenGeneratorService(IOptions<JwtSettings> jwtSettingsOptions) : IAccessTokenGeneratorService
    {
        private JwtSettings jwtSettings = jwtSettingsOptions.Value;

        public string GenerateToken(User user)
        {
            var claims = GetClaims(user);

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

            var credentials = new SigningCredentials(
                key: securityKey,
                algorithm: SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtSettings.ValidIssuer,
                audience: jwtSettings.ValidAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationTimeInMinutes),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        List<Claim> GetClaims(User user) =>
            new List<Claim>
            {
                new Claim(ClaimConstants.UserId, user.Id.ToString()),
                new Claim(ClaimConstants.Email, user.Email),
                new Claim(ClaimConstants.FirstName, user.FirstName),
                new Claim(ClaimConstants.LastName, user.LastName),
                new Claim(ClaimConstants.Job, user.Job),
                new Claim(ClaimConstants.Role, user.Role.ToString())
            };
    }
}
