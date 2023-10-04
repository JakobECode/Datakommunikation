using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

class Program
{
	static void Main()
	{
		// Skapa en UDP-mottagare och lyssna på portnummer 5000
		UdpClient udpListener = new UdpClient(5000);
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5000);

		try
		{
			Console.WriteLine("Väntar på data från klienten...");

			// Lyssna på inkommande data
			byte[] receivedData = udpListener.Receive(ref endPoint);

			// Konvertera byte-array till JSON
			var json = Encoding.UTF8.GetString(receivedData);

			// Deserialisera JSON till användarobjekt
			User user = JsonConvert.DeserializeObject<User>(json);

			Console.WriteLine("Data mottagen från klienten:");
			Console.WriteLine("Användarnamn: " + user.Username);
			Console.WriteLine("Ålder: " + user.Age);
		}
		catch (Exception e)
		{
			Console.WriteLine("Fel vid mottagning: " + e.Message);
		}
		finally
		{
			udpListener.Close();
		}
	}
}

// Användarobjekt som deserialiseras från JSON
class User
{
	public string Username { get; set; }
	public int Age { get; set; }
}
