using System;
using System.Threading.Tasks;
using api.dto.userDto;
using api.model;
using api.service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.controller;

[Route("api/account")]
[ApiController]
public class AppUserController : ControllerBase
{
	private readonly UserManager<AppUser> _userManager;
	private readonly ITokenService _tokenService;
	private readonly SignInManager<AppUser> _signInManager;
	
	public AppUserController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
	{
		_userManager = userManager;
		_tokenService = tokenService;
		_signInManager = signInManager;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			
			var appUser = new AppUser
			{
				UserName = registerDto.Username,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber
			};
			
			var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
			if (createdUser.Succeeded)
			{
				var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
				if (roleResult.Succeeded)
				{
					return Ok(new NewUserDto
					{
						Role = "user",
						Username = appUser.UserName,
						Token = _tokenService.CreateToken(appUser, isAdmin: false)
					});
				}
				else
				{
					return StatusCode(500, roleResult.Errors);
				}
			}
			else 
			{
				return BadRequest(createdUser.Errors);
			}
			
		}
		catch (Exception e)
		{
			return StatusCode(500, e);
		}
	}
	
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		
		var user = loginDto.Username.Contains("@") ? await _userManager.FindByEmailAsync(loginDto.Username) : await _userManager.FindByNameAsync(loginDto.Username);
		if (user == null)
		{
			return Unauthorized("Username not found and/or password incorrect");
		}
		
		var isUser = await _userManager.IsInRoleAsync(user, "User");
		if (!isUser)
		{
			return Unauthorized("Username not found and/or password incorrect");
		}
		
		var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
		if (!result.Succeeded)
		{
			return Unauthorized("Username not found and/or password incorrect");
		}

		return Ok(new NewUserDto
		{
			Role = "user",
			Username = user.UserName,
			Token = _tokenService.CreateToken(user, isAdmin: false)
		});
	}
	
	[HttpGet("checkUsername/{username}")]
	public async Task<IActionResult> CheckUsername(string username)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == username);
		return Ok(user != null);
	}
	
	[HttpGet("checkEmail/{email}")]
	public async Task<IActionResult> CheckEmail(string email)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
		return Ok(user != null);
	}
}