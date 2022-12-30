using System;
namespace RealTimeChat.Models
{
	public class UserInfoDto
	{
		public string Username { get; set; } = "";
		public string ImageUrl { get; set; } = "";
		public List<string> Friends { get; set; } = new List<string>();


        public UserInfoDto()
        {

        }

        public UserInfoDto(string username, string imageUrl, List<string> friends)
		{
			Username = username;
			ImageUrl = imageUrl;
			Friends = friends;
		}
	}
}

