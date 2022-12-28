using System;
using MongoDB.Driver;
using RealTimeChat.Models;
using Microsoft.Extensions.Options;


namespace RealTimeChat.Services;

public class DatabaseService
{
	private readonly IMongoCollection<User> _usersCollection;
    private readonly IMongoCollection<Chat> _chatsCollection;

    public DatabaseService(IOptions<DatabaseSettings> databaseSettings)

	{
		MongoClient client = new MongoClient(databaseSettings.Value.ConnectionString);
		IMongoDatabase db = client.GetDatabase(databaseSettings.Value.DatabaseName);
		_usersCollection = db.GetCollection<User>(databaseSettings.Value.UsersCollectionName);
        _chatsCollection = db.GetCollection<Chat>(databaseSettings.Value.ChatsCollectionName);

    }

	public async Task<Chat> CreateChatAsync(Chat chat)
	{
		await _chatsCollection.InsertOneAsync(chat);
		Console.WriteLine(chat.Id);
		return chat;
	}

	public async Task<Chat> GetChatAsync(string chatId)
	{
		var filter = Builders<Chat>.Filter.Eq(doc => doc.Id, chatId);
		var chats = await _chatsCollection.FindAsync(filter);
		return chats.FirstOrDefault();
	}

	public async Task SendMessageAsync(string chatId, Message message)
	{
        var filter = Builders<Chat>.Filter.Eq(doc => doc.Id, chatId);
		var update = Builders<Chat>.Update.Push(doc => doc.Messages, message);
		await _chatsCollection.FindOneAndUpdateAsync(filter, update);
		return;
    }

	public async Task DeleteChatAsync(string chatId)
	{
        var filter = Builders<Chat>.Filter.Eq(doc => doc.Id, chatId);
		await _chatsCollection.FindOneAndDeleteAsync(filter);
        return;
	}
}

