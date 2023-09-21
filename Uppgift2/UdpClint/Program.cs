using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class MyData
{
	public string Name { get; set; }
	public int Age { get; set; }
}

class Client2
{
	static async Task Main(string[] args)
	{
		int localPort = 1111;  // Lokal port
		int remotePort = 2222; // Port till Client1
		string remoteIPAddress = "127.0.0.1"; // Localhost ipadress

		UdpClient udpClient = new UdpClient(localPort);
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort);

		// Tar emot och visar meddelanden från Client1
		Task receiveTask = Task.Run(async () =>
		{
			while (true)
			{
				UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();
				byte[] data = receiveResult.Buffer;

				// Deserialisera objektet från byte-array
				string jsonData = Encoding.ASCII.GetString(data);
				MyData receivedData = JsonConvert.DeserializeObject<MyData>(jsonData);

				Console.WriteLine("Them: Name=" + receivedData.Name + ", Age=" + receivedData.Age);
			}
		});

		try
		{
			while (true)
			{
				Console.WriteLine("Press Enter to send the object...");
				Console.ReadLine(); // Vänta på Enter-tangenttryckning

				// Skapa ett objekt att skicka
				MyData myData = new MyData
				{
					Name = "John",
					Age = 30
				};

				// Serialisera objektet till JSON
				string jsonData = JsonConvert.SerializeObject(myData);
				byte[] data = Encoding.ASCII.GetBytes(jsonData);

				await udpClient.SendAsync(data, data.Length, endPoint);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("Error: " + e.Message);
		}
	}
}
