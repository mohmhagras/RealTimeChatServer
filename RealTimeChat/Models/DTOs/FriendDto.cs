using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
namespace RealTimeChat.Models
{
	public class FriendDto
	{
		[BsonElement("username")]
		public string Username { get; set; } = "";

        [BsonElement("image")]
        public string ImageUrl { get; set; } = "";

		public FriendDto(string username, string imageurl)
		{
			Username = username;
			ImageUrl = imageurl;
		}
	}
}

