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

	public async Task<User> GetUserAsyncById(string id)
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

	public async Task<UserInfoDto> GetInfoAsync()
	{
		string id = GetUserIdFromHttpContext();
		if (id == "") return new UserInfoDto();
		User user = await GetUserAsyncById(id);
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

}

