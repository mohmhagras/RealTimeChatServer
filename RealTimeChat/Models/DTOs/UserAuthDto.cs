using System;
namespace RealTimeChat.Models;
//used in login & registration
public class UserAuthDto
{
	public string Username { get; set; } = "";
	public string Password { get; set; } = "";
	public string ImageUrl { get; set; } = "";
}

