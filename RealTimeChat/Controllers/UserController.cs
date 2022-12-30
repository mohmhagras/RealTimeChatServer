using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Services;
using RealTimeChat.Models;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
}

