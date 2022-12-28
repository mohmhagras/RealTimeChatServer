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

	[BsonElement("user1id")]
	public string User1Id { get; set; }

    [BsonElement("user2id")]
    public string User2Id { get; set; }

	[BsonElement("messages")]
	public List<Message> Messages { get; set; } = new List<Message>();

	public Chat(string user1id, string user2id)
	{
		User1Id = user1id;
		User2Id = user2id;
	}

	public void SaveMessage(Message message)
	{
		Messages.Add(message);
            var filter = Builders<Chat>.Filter.Eq(doc => doc.User1Id, User1Id);
            var update = Builders<Chat>.Update.Set(doc => doc.Messages, Messages);
		Global.ChatsCollection.FindOneAndUpdate(filter, update);
        }
}

