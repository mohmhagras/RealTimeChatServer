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
	public string ImageUrl { get; set; }

	[BsonElement("friends")]
	public List<string> Friends { get; set; } = new List<string>(); //list of friend usernames

	public User(string username, string password)
	{
		Username = username;
		Password = password;
	}

}

