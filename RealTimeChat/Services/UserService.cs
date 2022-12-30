using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealTimeChat.Models;
using System.Security.Claims;

namespace RealTimeChat.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;
	private readonly IHttpContextAccessor _httpContextAccessor;


    public UserService(IOptions<DatabaseSettings> databaseSettings, IHttpContextAccessor httpContextAccessor)
	{
		IMongoClient mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
		IMongoDatabase database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
		_usersCollection = database.GetCollection<User>(databaseSettings.Value.UsersCollectionName);
		_httpContextAccessor = httpContextAccessor;
	}

	public async Task<User> GetAsyncById(string id)
	{
		var filter = Builders<User>.Filter.Eq(doc => doc.Id, id);
		var userQuery = await _usersCollection.FindAsync(filter);
		return userQuery.FirstOrDefault();
	}

	public string GetUserIdFromHttpContext()
	{
        if (_httpContextAccessor.HttpContext != null)
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        else
        {
            return "";
        }
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

    public async Task<UserInfoDto> GetInfoAsync()
	{
		string id = GetUserIdFromHttpContext();
		if (id == "") return new UserInfoDto();
		User user = await GetAsyncById(id);
		UserInfoDto userInfo = new UserInfoDto(user.Username, user.ImageUrl, user.Friends);

		return userInfo;
	}

	public async Task SetProfileImageAsync(string imageUrl)
	{
        string id = GetUserIdFromHttpContext();
        if (id == "") return;

        var filter = Builders<User>.Filter.Eq(doc => doc.Id, id);
		var update = Builders<User>.Update.Set(doc => doc.ImageUrl, imageUrl);
		await _usersCollection.FindOneAndUpdateAsync(filter, update);

		return;
    }

	public async Task SendFriendRequestAsync(string sentToUsername)
	{
		UserInfoDto sentFromUser = await GetInfoAsync();

		//TODO: implement promise.all like syntax here:

        var requestReceiverFilter = Builders<User>.Filter.Eq(doc => doc.Username, sentToUsername);
		var requestReceiverUpdate = Builders<User>.Update.Push(doc => doc.FriendRequestsRecieved, sentFromUser);
		await _usersCollection.FindOneAndUpdateAsync(requestReceiverFilter, requestReceiverUpdate);

        var requestSenderFilter = Builders<User>.Filter.Eq(doc => doc.Username, sentFromUser.Username);
        var requestSenderUpdate = Builders<User>.Update.Push(doc => doc.FriendRequestsSent, sentToUsername);
        await _usersCollection.FindOneAndUpdateAsync(requestSenderFilter, requestSenderUpdate);

        return;
    }

	public async Task AcceptFriendRequestAsync(UserInfoDto acceptedFriend, string acceptorImgUrl)
	{
        string id = GetUserIdFromHttpContext();
		string username = GetUsernameFromHttpContext();
        if (id == "") return;
		FriendDto user = new FriendDto(username, acceptorImgUrl); //acceptor object
		FriendDto friend = new FriendDto(acceptedFriend.Username, acceptedFriend.ImageUrl); //accepted object

		//TODO: implement promise.all like syntax here:
        var requestAcceptorFilter = Builders<User>.Filter.Eq(doc => doc.Id, id);
		var requestAcceptorUpdate = Builders<User>.Update.Push(doc => doc.Friends, friend).Pull(doc=> doc.FriendRequestsRecieved, acceptedFriend);
        await _usersCollection.FindOneAndUpdateAsync(requestAcceptorFilter, requestAcceptorUpdate);

        var requestSenderFilter = Builders<User>.Filter.Eq(doc => doc.Username, acceptedFriend.Username);
        var requestSenderUpdate = Builders<User>.Update.Push(doc => doc.Friends, user).Pull(doc => doc.FriendRequestsSent, acceptedFriend.Username);
		await _usersCollection.FindOneAndUpdateAsync(requestSenderFilter, requestSenderUpdate);
    }

}

