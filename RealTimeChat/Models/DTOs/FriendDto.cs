using System;
namespace RealTimeChat.Models
{
	public class FriendDto
	{
		public string Username { get; set; } = "";

		public string ImageUrl { get; set; } = "";

		public FriendDto(string username, string imageurl)
		{
			Username = username;
			ImageUrl = imageurl;
		}
	}
}

