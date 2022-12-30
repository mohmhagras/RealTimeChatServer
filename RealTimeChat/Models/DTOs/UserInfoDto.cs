using System;
namespace RealTimeChat.Models
{
	public class UserInfoDto
	{
		public string Username { get; set; } = "";
		public string ImageUrl { get; set; } = "";
		public List<FriendDto> Friends { get; set; } = new List<FriendDto>();


        public UserInfoDto()
        {

        }

        public UserInfoDto(string username, string imageUrl, List<FriendDto> friends)
		{
			Username = username;
			ImageUrl = imageUrl;
			Friends = friends;
		}
	}
}

