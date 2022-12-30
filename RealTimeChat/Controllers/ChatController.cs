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
		return await _chatService.GetChatAsync(chatId);
	}

	[HttpPost]
	public async Task<Chat> CreateChat([FromBody] Chat chat)
	{
		return await _chatService.CreateChatAsync(chat);
		
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
		await _chatService.DeleteChatAsync(chatId);
		return Ok();
	}


}

