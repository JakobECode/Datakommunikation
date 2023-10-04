using Newtonsoft.Json;
using System.Data;
using System.Net.WebSockets;
using System.Text;

class Program
{
	static async Task Main(string[] args)
	{
		using (ClientWebSocket clientWebSocket = new ClientWebSocket())
		{
			try
			{
				Uri serverUri = new Uri("ws://localhost:2000");
				await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
				Console.WriteLine("Ansluten till servern.");

				User user = new User { Username = "Emil", Age = 11 };

				// Skicka användarobjekt till servern
				string json = JsonConvert.SerializeObject(user);
				byte[] buffer = Encoding.UTF8.GetBytes(json);
				await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

				// Ta emot svar från servern
				buffer = new byte[1024];
				WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

				User serverResponse = JsonConvert.DeserializeObject<User>(receivedMessage)!;
				Console.WriteLine($"Svar från servern: {serverResponse.Username}, Ålder: {serverResponse.Age}");

				await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "klient stänger", CancellationToken.None);
			}
			catch (WebSocketException ex)
			{
				Console.WriteLine($"WebSocket-fel client: {ex.Message}");
			}
		}
	}
}
class User
{
	public string Username { get; set; } = null!;
	public int Age { get; set; }
}