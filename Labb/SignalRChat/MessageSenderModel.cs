using ASPSignalR.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat
{
	public class MessageSenderModel : PageModel
	{
		private readonly IHubContext<ChatHub> _hubContext;
		private readonly ILogger<MessageSenderModel> _logger;

		public MessageSenderModel(IHubContext<ChatHub> hubContext, ILogger<MessageSenderModel> logger)
		{
			_hubContext = hubContext;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", "Message sent from Razor Page!");
			_logger.LogInformation("Message sent from Razor Page at {Time}", DateTime.UtcNow);
			return Page();
		}
	}
}
