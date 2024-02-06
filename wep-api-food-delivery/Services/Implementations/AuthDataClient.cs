﻿using Grpc.Net.Client;
using web_api_food_delivery;
using wep_api_food_delivery.Services.Intefaces;

namespace wep_api_food.Services.Implementations
{
    public class AuthDataClient : IAuthDataClient
    {
        private readonly IConfiguration _configuration;

        public AuthDataClient(IConfiguration configuration) 
        {
            _configuration = configuration; 
        }

        public async Task<string> ReturnTokenAsync(string email, string role)
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcAuth"]);
            var client = new GrpcToken.GrpcTokenClient(channel);
            var request = new CreateTokenRequest()
            {
                Email = email,
                Role = role,
                Audience = _configuration["Audience"],
                Key = _configuration["SecretKey"]
            };
            try
            {
                var reply = await client.CreateTokenAsync(request);
                return reply.Token;
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"--> Couldnot call GRPC Server {ex.Message}");
                return null;
            }
        }
    }
}
