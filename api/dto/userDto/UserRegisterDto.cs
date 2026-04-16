using System.ComponentModel.DataAnnotations;

namespace api.dto.userDto;

public class UserRegisterDto
{
	[Required]
	[MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
	public string? Username { get; set; }
	
	[Required]
	[EmailAddress]
	public string? Email { get; set; }
	
	[Required]
	[Phone]
	public string? PhoneNumber { get; set; }
	
	[Required]
	public string? Password { get; set; }
}