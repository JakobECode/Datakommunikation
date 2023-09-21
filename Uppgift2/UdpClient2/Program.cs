using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Client2
{
	static async Task Main(string[] args)
	{
		int localPort = 2222;  // Lokal port
		int remotePort = 1111; // Port till Client1
		string remoteIPAddress = "127.0.0.1"; //Localhost ipadress

		UdpClient udpClient = new UdpClient(localPort);
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort);

		// Tar emot och visar meddelanden från Client1
		Task receiveTask = Task.Run(async () =>
		{
			while (true)
			{
				UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();
				byte[] data = receiveResult.Buffer;
				string message = Encoding.ASCII.GetString(data);
				Console.WriteLine("Them: " + message);
			}
		});

		try
		{
			while (true)
			{
				string message = Console.ReadLine()!;

				if (!string.IsNullOrEmpty(message))
				{
					byte[] data = Encoding.ASCII.GetBytes(message);
					await udpClient.SendAsync(data, data.Length, endPoint);
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("Error: " + e.Message);
		}
	}
}