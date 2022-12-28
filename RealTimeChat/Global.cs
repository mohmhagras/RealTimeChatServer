using System;
using MongoDB.Driver;
using RealTimeChat.Models;
namespace RealTimeChat
{
	static class Global
	{
        public static MongoDatabaseBase Database;
        public static MongoCollectionBase<User> UsersCollection;
        public static MongoCollectionBase<Chat> ChatsCollection;
    }
}

