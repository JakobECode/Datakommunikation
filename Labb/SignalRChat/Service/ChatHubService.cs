namespace SignalRChat.Service
{
	public class ChatHubService : IChatHubService
	{
		public async Task<bool>ProcessMessageAsync(string user, string message) 
		{
			await Task.Delay(100);
			return true;
		}

		public async Task<bool> ProcessPrivateMessageAsync(string gruopName, string user, string message)
		{

			await Task.Delay(100);
			return true;
		}
	}
}
