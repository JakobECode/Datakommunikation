namespace SignalRChat.Service
{
    public interface IChatHubService
    {
        Task<bool> ProcessMessageAsync(string user, string message);
        Task<bool> ProcessPrivateMessageAsync(string groupName, string user, string message);
    }
}
