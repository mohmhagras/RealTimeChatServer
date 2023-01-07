using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
namespace RealTimeChat.Models;

public class UserInfoDto
{
    [BsonElement("username")]
    public string Username { get; set; } = "";

    [BsonElement("image")]
    public string ImageUrl { get; set; } = "";

    [BsonElement("friends")]
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

