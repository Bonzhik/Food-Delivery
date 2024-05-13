using Grpc.Core;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace web_api_food_auth.Services
{
    public class GrpcTokenService : GrpcToken.GrpcTokenBase
    {
        private readonly IConfiguration _configuration;

        public GrpcTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public override Task<CreateTokenResponse> CreateToken(CreateTokenRequest request, ServerCallContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var _key = Encoding.UTF8.GetBytes(request.Key);

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, request.Email),
                new Claim(ClaimTypes.Role, request.Role)
            });

            var tokenDescritor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Issuer = _configuration["Issuer"],
                Audience = request.Audience,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescritor);
            var stringToken = "Bearer " + tokenHandler.WriteToken(token);
            var response = new CreateTokenResponse
            {
                Token = stringToken

            };

            return Task.FromResult(response);
        }
    }
}
