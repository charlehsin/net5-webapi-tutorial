using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TodoApi.Authentication
{
    public class JwtAuth: IJwtAuth
    {
        private readonly string _key;

        public JwtAuth(string key)
        {
            _key = key;
        }

        public string GetToken(string userName, IList<string> roles)
        {
            var jwtSecutiryTokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(_key);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName)
                };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    authClaims,
                    JwtBearerDefaults.AuthenticationScheme),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtSecutiryTokenHandler.CreateToken(securityTokenDescriptor);

            return jwtSecutiryTokenHandler.WriteToken(token);
        }
    }
}