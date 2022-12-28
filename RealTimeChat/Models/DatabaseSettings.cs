using System;
namespace RealTimeChat.Models;

	public class DatabaseSettings
	{
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;

        public string ChatsCollectionName { get; set; } = null!;
    }

