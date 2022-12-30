using System;
using MongoDB.Driver;
using RealTimeChat.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace RealTimeChat.Services;

public class AuthService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly string _token;

    public AuthService(IOptions<DatabaseSettings> databaseSettings,IOptions<AuthConfiguration> authConfiguration)
    {
        MongoClient client = new MongoClient(databaseSettings.Value.ConnectionString);
        IMongoDatabase db = client.GetDatabase(databaseSettings.Value.DatabaseName);
        _usersCollection = db.GetCollection<User>(databaseSettings.Value.UsersCollectionName);
        _token = authConfiguration.Value.Token;
    }

    public async Task RegisterAsync(UserDto request)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        User user = new User(request.Username, hashedPassword);
        await _usersCollection.InsertOneAsync(user);
        return;
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_token));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.Now.AddDays(30)
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public async Task<string> LoginAsync(UserDto request)
    {
        var filter = Builders<User>.Filter.Eq(doc => doc.Username, request.Username);
        var userQuery = await _usersCollection.FindAsync(filter);
        User user = userQuery.FirstOrDefault();
        if(user==null)
        {
            return "0";
        }
        if(BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return CreateToken(user);
        }

        return "1";
    }


}

