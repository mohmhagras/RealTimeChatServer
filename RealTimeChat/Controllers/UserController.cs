using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Services;
using RealTimeChat.Models;
using Newtonsoft.Json.Linq;

namespace RealTimeChat.Controllers;

[Controller]
[Route("api/[controller]")]
[Authorize]


public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<UserInfoDto> GetUserInfo()
    {
        return await _userService.GetInfoAsync();
    }

    [HttpGet("friendrequests")]
    public async Task<List<UserInfoDto>> GetFriendRequests()
    {
        return await _userService.GetFriendRequests();
    }

    [HttpPost("setimage")]
    public async Task<IActionResult> SetProfileImage([FromBody] UserImageDto data)
    {
        await _userService.SetProfileImageAsync(data.ImageUrl);

        return CreatedAtAction(nameof(SetProfileImage), data);
    }

    [HttpPost("sendrequest")]
    public async Task<IActionResult> SendFriendRequest([FromBody] string sentTo)
    {
        var response  = await _userService.SendFriendRequestAsync(sentTo);

        if (response == 200)
        {
            return CreatedAtAction(nameof(SendFriendRequest), new { sentTo });
        }
        else if(response == 0)
        {
            return BadRequest("Can't get user ID");

        }
        else if (response == 1)
        {
            return BadRequest("User is already a friend!");

        }
        else if (response == 2)
        {
            return BadRequest("Friend Request already sent!");

        }


        return BadRequest("User does not exist!");
    }

    [HttpPut("acceptrequest")]
    public async Task<IActionResult> AcceptFriendRequest([FromBody] UserInfoDto acceptedFriend)
    {
        await _userService.AcceptFriendRequestAsync(acceptedFriend);

        return CreatedAtAction(nameof(AcceptFriendRequest), acceptedFriend);
    }


}