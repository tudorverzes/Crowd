using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace api.model;

public class AppUser : IdentityUser
{
	public List<Permission> Permissions { get; set; } = [];
	public List<Ticket> Tickets { get; set; } = [];
	public List<Report> Reports { get; set; } = [];
	public List<Ad> Ads { get; set; } = [];
}