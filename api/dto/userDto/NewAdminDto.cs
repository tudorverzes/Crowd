using System.ComponentModel.DataAnnotations;

namespace api.dto.userDto;

public class NewAdminDto
{
	[Required]
	public string? Username { get; set; }
}