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
        await Clients.Users(receiver, Context?.User?.Identity?.Name!).SendAsync("receiveMessage", message, receiver);
        await _chatService.SendMessageAsync(receiver, Context?.User?.Identity?.Name!, message);
        //await Clients.All.SendAsync("sendMessage", message);
        return;
    }
}

