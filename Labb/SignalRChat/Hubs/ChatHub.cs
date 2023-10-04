using Microsoft.AspNetCore.SignalR;
using SignalRChat.Service;
using System.Text.RegularExpressions;

namespace ASPSignalR.Hubs
{
	public class ChatHub : Hub
	{
		private readonly IChatHubService _chatHubService;
		
		public ChatHub(IChatHubService chatHubService)
		{
			_chatHubService = chatHubService;
		}

		// Denna metod körs när en klient ansluter till hubben
		public override async Task OnConnectedAsync()
		{
			Console.WriteLine("ConnectionID: {0}", Context.ConnectionId);
		}

		// Denna metod låter en klient ansluta till en grupp
		public async Task AddToGroup(string groupName)
		{
			Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			Console.WriteLine("Joined a new client to group {0}", groupName);
			await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the group {groupName}.");
		}

		// Denna metod används för att skicka ett meddelande till alla klienter i hubben
		public async Task SendMessage(string user, string message)
		{
			var processed = await _chatHubService.ProcessMessageAsync(user, message);
			if (processed)
			{
				await Clients.All.SendAsync("ReceiveMessage", user, message);
			}
		}

		// Denna metod används för att skicka ett privat meddelande till en grupp av klienter
		public async Task SendPrivateMessage(string groupName, string user, string message)
		{
			var processed = await _chatHubService.ProcessPrivateMessageAsync(groupName, user, message);
			if (processed)
			{
				await Clients.Group(groupName).SendAsync("ReceivePrivateMessage", user, message);
			}
		}
	}
}
