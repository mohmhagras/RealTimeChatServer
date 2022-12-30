using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

using System.Net.NetworkInformation;


namespace RealTimeChat.Models;

public class Chat
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }

	[BsonElement("usernames")]
	public string[] Usernames { get; set; } = new string[2];

	[BsonElement("messages")]
	public List<Message> Messages { get; set; } = new List<Message>();

	public Chat(string[] usernames)
	{
        Usernames = usernames;
	}
}

