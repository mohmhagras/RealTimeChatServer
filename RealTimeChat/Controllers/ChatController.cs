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
	private readonly DatabaseService _databaseService;


	public ChatController(DatabaseService databaseService)
	{
		_databaseService = databaseService;
	}

	[HttpGet("{chatId}")]
	public async Task<Chat> GetChat(string chatId)
	{
		return await _databaseService.GetChatAsync(chatId);
	}

	[HttpPost]
	public async Task<Chat> CreateChat([FromBody] Chat chat)
	{
		return await _databaseService.CreateChatAsync(chat);
		
	}

	[HttpPut("{chatId}")]
	public async Task<IActionResult> SendMessage(string chatId, [FromBody] Message message)
	{
		await _databaseService.SendMessageAsync(chatId, message);

		return CreatedAtAction(nameof(SendMessage), message);
	}

	[HttpDelete("{chatId}")]
	public async Task<IActionResult> DeleteChat(string chatId)
	{
		await _databaseService.DeleteChatAsync(chatId);
		return Ok();
	}


}

