using MongoDB.Driver;
using RealTimeChat.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace RealTimeChat.Services;

public class AuthService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly string _token;

    public AuthService(IOptions<DatabaseSettings> databaseSettings, IOptions<AuthConfiguration> authConfiguration, IMongoDatabase database)
    {
        _usersCollection = database.GetCollection<User>(databaseSettings.Value.UsersCollectionName);

        // security key used in creating the jwt
        _token = authConfiguration.Value.Token; 

        //creating a unique index for the username field
        var keys = Builders<User>.IndexKeys.Ascending("username");
        var indexOptions = new CreateIndexOptions { Unique = true };
        var model = new CreateIndexModel<User>(keys, indexOptions);
        _usersCollection.Indexes.CreateOne(model);
    }




        public async Task<int> RegisterAsync(UserAuthDto request)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        User user = new User(request.Username, request.ImageUrl, hashedPassword);
        try
        {
            await _usersCollection.InsertOneAsync(user);
        }
        catch(Exception)
        {
           return 0;
        }
        return 1;

    }


    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
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

    public async Task<string> LoginAsync(UserAuthDto request)
    {
        var filter = Builders<User>.Filter.Eq(doc => doc.Username, request.Username);
        var userQuery = await _usersCollection.FindAsync(filter);
        User user = userQuery.FirstOrDefault();

        if(user == null)
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

