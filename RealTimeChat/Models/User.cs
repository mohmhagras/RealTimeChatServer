using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace RealTimeChat.Models;

public class User
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }

	[BsonElement("username")]
	public string Username { get; set; }

	[BsonElement("password")]
	public string Password { get; set; }

	[BsonElement("image")]
	public string ImageUrl { get; set; } = "";

	[BsonElement("friends")]
	public List<FriendDto> Friends { get; set; } = new List<FriendDto>(); //list of friend usernames

	[BsonElement("friendRequests")]
	public List<UserInfoDto> FriendRequestsRecieved { get; set; } = new List<UserInfoDto>();

	[BsonElement("friendRequestsSent")]
	public List<string> FriendRequestsSent { get; set; } = new List<string>();

	public User(string username, string password)
	{
		Username = username;
		Password = password;
	}

}

