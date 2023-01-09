using System;
using MongoDB.Driver;
using RealTimeChat.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace RealTimeChat.Services;

public class ChatService
{
    private readonly IMongoCollection<Chat> _chatsCollection;
	private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatService(IOptions<DatabaseSettings> databaseSettings, IHttpContextAccessor httpContextAccessor, IMongoDatabase database)
	{
        _chatsCollection = database.GetCollection<Chat>(databaseSettings.Value.ChatsCollectionName);
		_httpContextAccessor = httpContextAccessor;
    }

    public string GetUsernameFromHttpContext()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
        else
        {
            return "";
        }
    }

    public async Task<List<Chat>> GetUserChats()
    {
        string username = GetUsernameFromHttpContext();
        if (username == "") return new List<Chat>();

        //sorting chats based on the DateTime of the last activty (activity = message sent or chat created)
        var filter = Builders<Chat>.Filter.AnyEq(doc => doc.Usernames, username);
        var sort = Builders<Chat>.Sort.Descending(doc => doc.LastActivity);
        var response = await _chatsCollection.FindAsync(filter, new FindOptions<Chat, Chat>()
        {
            Sort = sort
        });

        return response.ToList();
    }


    public async Task<Chat> CreateAsync(Chat chat)
	{
		await _chatsCollection.InsertOneAsync(chat);
		return chat;
	}

	public async Task<Chat> GetAsync(string chatId)
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
		var filter = Builders<Chat>.Filter.Where(doc => doc.Usernames.Contains<string>(username1) && doc.Usernames.Contains<string>(username2));
        var update = Builders<Chat>.Update.Push(doc => doc.Messages, message).Set(doc => doc.LastActivity, message.SentAt);
        await _chatsCollection.FindOneAndUpdateAsync(filter, update);
        return;
    }

    public async Task DeleteAsync(string chatId)
	{
        var filter = Builders<Chat>.Filter.Eq(doc => doc.Id, chatId);
		await _chatsCollection.FindOneAndDeleteAsync(filter);
        return;
	}

    /*
     * This method was used to add the lastActivity property to all existing chats in the DB, as it wasn't added from the start.
    public async Task SetLastActivity()
    {
        var filter = Builders<Chat>.Filter.Empty;
        var chats = await _chatsCollection.FindAsync<Chat>(filter);
        List<Chat> chatsList = chats.ToList();

        foreach(Chat item in chatsList)
        {
            var filter2 = Builders<Chat>.Filter.Eq(doc => doc.Id, item.Id);
            var update2 = Builders<Chat>.Update.Set(doc => doc.LastActivity, item.Messages.LastOrDefault(new Message(DateTime.Now)).SentAt);
            await _chatsCollection.FindOneAndUpdateAsync<Chat>(filter2, update2);
        }

        return;
    }
    */
}

