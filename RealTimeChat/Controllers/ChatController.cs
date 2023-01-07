using System;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Services;
using RealTimeChat.Models;
using Microsoft.AspNetCore.Authorization;

namespace RealTimeChat.Controllers;


[Controller]
[Route("api/[controller]")]
[Authorize]
public class ChatController : Controller
{
	private readonly ChatService _chatService;


	public ChatController(ChatService chatService)
	{
        _chatService = chatService;
	}

	[HttpGet("{chatId}")]
	public async Task<Chat> GetChat(string chatId)
	{
		return await _chatService.GetAsync(chatId);
	}

    [HttpGet("getuserchats")]
    public async Task<List<Chat>> GetChats()
    {
        return await _chatService.GetUserChats();
    }

    [HttpPost]
	public async Task<Chat> CreateChat([FromBody] Chat chat)
	{
		return await _chatService.CreateAsync(chat);
		
	}

	[HttpPut("{chatId}")]
	public async Task<IActionResult> SendMessage(string chatId, [FromBody] Message message)
	{
		await _chatService.SendMessageAsync(chatId, message);

		return CreatedAtAction(nameof(SendMessage), message);
	}

	[HttpDelete("{chatId}")]
	public async Task<IActionResult> DeleteChat(string chatId)
	{
		await _chatService.DeleteAsync(chatId);
		return Ok();
	}


}

