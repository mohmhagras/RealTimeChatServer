using System;
using MongoDB.Driver;
using RealTimeChat.Models;
using Microsoft.Extensions.Options;


namespace RealTimeChat.Services;

public class ChatService
{
    private readonly IMongoCollection<Chat> _chatsCollection;
	private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatService(IOptions<DatabaseSettings> databaseSettings, IHttpContextAccessor httpContextAccessor)
	{
		MongoClient client = new MongoClient(databaseSettings.Value.ConnectionString);
		IMongoDatabase db = client.GetDatabase(databaseSettings.Value.DatabaseName);
        _chatsCollection = db.GetCollection<Chat>(databaseSettings.Value.ChatsCollectionName);
		_httpContextAccessor = httpContextAccessor;
    }

	public async Task<Chat> CreateChatAsync(Chat chat)
	{
		await _chatsCollection.InsertOneAsync(chat);
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

    public async Task SendMessageAsync(string username1, string username2, Message message)
    {
		var filter = Builders<Chat>.Filter.Where(doc => (doc.User1Id == username1 || doc.User1Id == username2) && (doc.User2Id == username1 || doc.User2Id == username2));
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

