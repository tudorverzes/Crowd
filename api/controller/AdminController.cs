using System.Threading.Tasks;
using api.dto.userDto;
using api.model;
using api.service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.controller;

[Route("api/admin")]
public class AdminController : ControllerBase
{
	private readonly UserManager<AppUser> _userManager;
	private readonly ITokenService _tokenService;
	private readonly SignInManager<AppUser> _signInManager;
	
	public AdminController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
	{
		_userManager = userManager;
		_tokenService = tokenService;
		_signInManager = signInManager;
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		
		var user = await _userManager.FindByNameAsync(loginDto.Username);
		if (user == null)
		{
			return Unauthorized("Username not found and/or password incorrect");
		}
		
		var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
		if (!isAdmin)
		{
			return Unauthorized("Username not found and/or password incorrect");
		}
		
		var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
		if (!signInResult.Succeeded)
		{
			return Unauthorized("Username not found and/or password incorrect");
		}
		
		return Ok(new NewUserDto
		{
			Role = "admin",
			Username = user.UserName,
			Token = _tokenService.CreateToken(user, isAdmin: true)
		});
	}
	
	[HttpPost]
	public async Task<IActionResult> MakeAdmin([FromBody] NewAdminDto newAdminDto)
	{
		// current user must be an admin
		var userId = HttpContext.User?.FindFirst("userId")?.Value;
		var userRole = HttpContext.User?.FindFirst("role")?.Value;
		if (userId == null || userRole == null)
		{
			return Unauthorized("You do not have permission to perform this action");
		}
		
		if (userRole != "admin")
		{
			return Unauthorized("You do not have permission to perform this action");
		}
		
		var databaseUser = await _userManager.FindByIdAsync(userId);
		if (databaseUser == null)
		{
			return NotFound("You do not have permission to perform this action");
		}
		
		var databaseRole = await _userManager.GetRolesAsync(databaseUser);
		if (databaseRole.Contains("Admin"))
		{
			return Unauthorized("You do not have permission to perform this action");
		}
		
		// create new admin
		var newAdminUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == newAdminDto.Username);
		if (newAdminUser == null)
		{
			return NotFound("User not found");
		}
		
		if (await _userManager.IsInRoleAsync(newAdminUser, "Admin"))
		{
			return BadRequest("User is already an admin");
		}
		else
		{
			await _userManager.AddToRoleAsync(newAdminUser, "Admin");
			return Ok();
		}
	}
}