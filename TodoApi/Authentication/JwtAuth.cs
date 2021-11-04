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
            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Create Private Key to Encrypted
            var tokenKey = Encoding.ASCII.GetBytes(_key);

            //3. Create JETdescriptor
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName)
                };
            foreach (var role in roles)  
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));  
            }
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    authClaims,
                    JwtBearerDefaults.AuthenticationScheme),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            
            //4. Create Token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 5. Return Token from method
            return tokenHandler.WriteToken(token);
        }
    }
}