using System;
using RealTimeChat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using RealTimeChat.Services;

namespace RealTimeChat.Hubs;

[Authorize]
public class ChatHub: Hub
{
    private readonly ChatService _chatService;


    public ChatHub(ChatService chatService)
    {
        _chatService = chatService;

    }

    public async Task SendMessage(string receiver, string text)
	{
		Message message = new Message(Context?.User?.Identity?.Name!, text);
        await Clients.Users(receiver, Context?.User?.Identity?.Name!).SendAsync("sendMessage", message);
        await _chatService.SendMessageAsync("63ac7669de08d9aeba250267", message);
        //await Clients.All.SendAsync("sendMessage", message);
        return;
    }
}

