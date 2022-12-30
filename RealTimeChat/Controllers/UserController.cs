using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Services;
using RealTimeChat.Models;

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

    [HttpPost]
    public async Task<IActionResult> SetProfileImage([FromBody] string imageUrl)
    {
        await _userService.SetProfileImageAsync(imageUrl);

        return CreatedAtAction(nameof(SetProfileImage), imageUrl);
    }

    [HttpPost]
    public async Task<IActionResult> SendFriendRequest(string sentTo)
    {
        await _userService.SendFriendRequestAsync(sentTo);

        return CreatedAtAction(nameof(SendFriendRequest), sentTo);
    }
}