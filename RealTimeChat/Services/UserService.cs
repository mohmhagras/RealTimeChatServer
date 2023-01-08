using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealTimeChat.Models;
using System.Security.Claims;
using System.Linq;

namespace RealTimeChat.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;
	private readonly IHttpContextAccessor _httpContextAccessor;


    public UserService(IOptions<DatabaseSettings> databaseSettings, IHttpContextAccessor httpContextAccessor, IMongoDatabase database)
	{
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



    public async Task<List<UserInfoDto>> GetFriendRequests()
	{
        string id = GetUserIdFromHttpContext();
        if (id == "") return new List<UserInfoDto>();
        User user = await GetAsyncById(id);
        return user.FriendRequestsRecieved;
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

	public async Task<int> SendFriendRequestAsync(string sentToUsername)
	{
        string id = GetUserIdFromHttpContext();
        if (id == "") return 0; // couldn't get user id
        User user = await GetAsyncById(id);

        bool isAlreadyAFriend = false;
        foreach (FriendDto friend in user.Friends)
        {
            if (friend.Username == sentToUsername) isAlreadyAFriend = true;
        }

        if (isAlreadyAFriend) return 1; // user already exists as a friend

        else if (user.FriendRequestsSent.Contains(sentToUsername))
        {
            return 2; //friend request already sent
        }

        UserInfoDto sentFromUser = new UserInfoDto(user.Username, user.ImageUrl, user.Friends);

        var requestReceiverFilter = Builders<User>.Filter.Eq(doc => doc.Username, sentToUsername);
		var requestReceiverUpdate = Builders<User>.Update.Push(doc => doc.FriendRequestsRecieved, sentFromUser);
		var reciever =  _usersCollection.FindOneAndUpdateAsync(requestReceiverFilter, requestReceiverUpdate);


        var requestSenderFilter = Builders<User>.Filter.Eq(doc => doc.Username, sentFromUser.Username);
        var requestSenderUpdate = Builders<User>.Update.Push(doc => doc.FriendRequestsSent, sentToUsername);
        var sender =  _usersCollection.FindOneAndUpdateAsync(requestSenderFilter, requestSenderUpdate);


        var tasksList = await Task.WhenAll(reciever, sender);
        if (tasksList[0] == null) return 3; // user does not exist


        return 200;
    }

	public async Task AcceptFriendRequestAsync(UserInfoDto acceptedFriend)
	{
        string id = GetUserIdFromHttpContext();
        User userData = await GetAsyncById(id);

        if (id == "") return;
		FriendDto user = new FriendDto(userData.Username, userData.ImageUrl); //acceptor object
		FriendDto friend = new FriendDto(acceptedFriend.Username, acceptedFriend.ImageUrl); //accepted object

        var requestAcceptorFilter = Builders<User>.Filter.Eq(doc => doc.Id, id);
		var requestAcceptorUpdate = Builders<User>.Update.Push(doc => doc.Friends, friend).Pull(doc=> doc.FriendRequestsRecieved, acceptedFriend);
        var requestAcceptor =  _usersCollection.FindOneAndUpdateAsync(requestAcceptorFilter, requestAcceptorUpdate);

        var requestSenderFilter = Builders<User>.Filter.Eq(doc => doc.Username, acceptedFriend.Username);
        var requestSenderUpdate = Builders<User>.Update.Push(doc => doc.Friends, user).Pull(doc => doc.FriendRequestsSent, acceptedFriend.Username);
		var requestSender =  _usersCollection.FindOneAndUpdateAsync(requestSenderFilter, requestSenderUpdate);

        await Task.WhenAll(requestAcceptor, requestSender);
        return;
    }



}

