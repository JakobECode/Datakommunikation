using System.Net.Sockets;
using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Server
{
	internal class Program
	{
		static void Main(string[] args)
		{
			TcpListener server = null;
			try
			{
				// Set the TcpListener on port.
				Int32 port = 1111;
				IPAddress localAddr = IPAddress.Parse("127.0.0.1");

				// TcpListener server = new TcpListener(port);
				server = new TcpListener(localAddr, port);

				// Start listening for client requests.
				server.Start();

				// Enter the listening loop.
				while (true)
				{
					Console.Write("Waiting for a connection... ");

					// Perform a blocking call to accept requests.
					// You could also use server.AcceptSocket() here.
					using TcpClient client = server.AcceptTcpClient();
					Console.WriteLine("Connected!");

					string data = null!;
					Byte[] bytes = new Byte[256];
					// Get a stream object for reading and writing
					NetworkStream stream = client.GetStream();

					int i;

					// Loop to receive all the data sent by the client.
					while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						// Translate data bytes to a ASCII string.
						data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

						Console.WriteLine("Received: {0}", data);
						BilTest received = JsonConvert.DeserializeObject<BilTest>(data)!;
						
						Console.WriteLine("Received BilTest Object: Brand = {0}, Color = {1}, Price = {2}",
									 	received.Brand, received.Color, received.Price);

						string responseMessage = "Received and processed the Shoe object.";

						byte[] msg = System.Text.Encoding.ASCII.GetBytes(responseMessage);

						// Send back a response.
						stream.Write(msg, 0, msg.Length);
						Console.WriteLine("Sent: {0}", responseMessage);
					}
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine("SocketException: {0}", e);
			}
			finally
			{
				server!.Stop();
			}

			Console.WriteLine("\nHit enter to continue...");
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