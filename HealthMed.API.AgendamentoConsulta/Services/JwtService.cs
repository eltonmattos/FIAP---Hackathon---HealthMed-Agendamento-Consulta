using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HealthMed.API.AgendamentoConsulta.Services
{
    public class JwtService(IConfiguration config)
    {
        public string? Secret { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpirationMinutes { get; set; }

        public readonly IConfiguration config = config;

        public string GenerateToken(string email, string role)
        {
            if (config == null)
            {
                throw new Exception(nameof(config));
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            Secret ??= config["JwtSettings:Secret"];

            if (Secret == null)
            {
                throw new Exception("Secret cannot be null");
            }

            Issuer ??= config["JwtSettings:Issuer"];
            if (Issuer == null)
            {
                throw new Exception("Issuer cannot be null");
            }

            Audience ??= config["JwtSettings:Audience"];
            if (Audience == null)
            {
                throw new Exception("Audience cannot be null");
            }

            var expirationMinutesString = config["JwtSettings:ExpirationMinutes"] ?? throw new Exception("ExpirationMinutes cannot be null");
            ExpirationMinutes = int.Parse(expirationMinutesString);

            byte[] key = Encoding.UTF8.GetBytes(Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                        new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Role, role)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}