using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace RealTimeChat.Models;

public class Message
{
	public string FromUser { get; set; } //msg sender username

        public DateTime SentAt { get; set; } = DateTime.Now;

        public string Text { get; set; }

	[BsonElement("status")]
	[BsonRepresentation(BsonType.String)]
	public MessageStatus Status { get; set; } = MessageStatus.SENT;

	public Message(string fromUser, string text)
	{
		FromUser = fromUser;
		Text = text;
	}

	public void UpdateStatus()
	{
		if(Status != MessageStatus.READ)
		{
                Status++;
            }
        }
}

