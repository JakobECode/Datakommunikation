using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Client
{
	internal class Program
	{
		static void Main(string[] args)
		{
			try
			{
				BilTest bil = new BilTest
				{
					Brand = "Volvo",
					Price = 55,
					Color = "Pink"
				};
				// Create a TcpClient.
				Int32 port = 1111;

				// Prefer a using declaration to ensure the instance is Disposed later.
				using TcpClient client = new TcpClient("127.0.0.1", port);
				string jsonBil = JsonConvert.SerializeObject(bil); 

				// Translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = System.Text.Encoding.ASCII.GetBytes(jsonBil);

				// Get a client stream for reading and writing.
				NetworkStream stream = client.GetStream();

				// Send the message to the connected TcpServer.
				stream.Write(data, 0, data.Length);

				Console.WriteLine("Sent: {0}", jsonBil);

				// Receive the server response.
				// Buffer to store the response bytes.
				data = new Byte[256];

				// String to store the response ASCII representation.
				String responseData = String.Empty;

				// Read the first batch of the TcpServer response bytes.
				Int32 bytes = stream.Read(data, 0, data.Length);
				responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Received: {0}", responseData);
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("ArgumentNullException: {0}", e);
			}
			catch (SocketException e)
			{
				Console.WriteLine("SocketException: {0}", e);
			}

			Console.WriteLine("\n Press Enter to continue...");
			Console.Read();
		}
	}

	public class BilTest
	{
        public string Color { get; set; } = null!;
		public int Price { get; set; }
		public string Brand { get; set; } = null!;
    }
}