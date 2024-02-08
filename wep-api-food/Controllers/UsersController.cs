﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wep_api_food.Dtos;
using wep_api_food.Enums;
using wep_api_food.Helpers.Hash;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;
using wep_api_food.Services.Intefaces;

namespace wep_api_food.Controllers
{
    [ApiController]
    [Route("api/pub/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthDataClient _authDataClient;

        public UsersController(IMapper mapper, 
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher, 
            IAuthDataClient authDataClient)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _authDataClient = authDataClient;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserCreateModel userDto)
        {
            if (userDto == null)
            {
                return BadRequest();
            }
            if (await _userRepository.IsExists(userDto.Email))
            {
                return Conflict();
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = userDto.Email,
                Password = _passwordHasher.HashPassword(userDto.Password),
                Role = Roles.User
            };

            if (! await _userRepository.Create(user))
            {
                return StatusCode(500, "Internal Server Error");
            }

            var token = await _authDataClient.ReturnTokenAsync(user.Email, user.Role.ToString());

            var userRead = new UserReadModel()
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                Password = userDto.Password
            };

            return Ok(userRead);
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(UserLoginModel userDto)
        {
            var user = await _userRepository.Get(userDto.Email);
            if (user == null)
            {
                return NotFound();
            }
            if (!_passwordHasher.VerifyPasswod(user.Password, userDto.Password)) {
                return BadRequest();
            }
            var token = await _authDataClient.ReturnTokenAsync(user.Email, user.Role.ToString());

            var userRead = new UserReadModel()
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                Password = userDto.Password
            };
            return Ok(userRead);
        }

    }
}
