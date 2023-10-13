using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using TempGenerator;

class Program
{
	// En konstant nyckel genereras vid körning för att kryptera temperaturdata.
	static byte[] key = {
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
};

	static byte[] iv = new byte[16]; // Initialization Vector. You can generate and store this if needed.

	static async Task Main(string[] args)
	{
		// Skapar en SignalR-anslutning till den angivna URL:en.
		var connection = new HubConnectionBuilder()
			.WithUrl("https://localhost:7045/TemperatureHub")
			.Build();

		// Funktionen som ska köras när anslutningen till SignalR-servern stängs.
		connection.Closed += async (error) =>
		{
			// Väntar en slumpmässig tid (mellan 0 och 5 sekunder) innan ett nytt försök att ansluta.
			await Task.Delay(new Random().Next(0, 5) * 1000);
			await connection.StartAsync();
		};

		// Startar SignalR-anslutningen.
		await connection.StartAsync();

		var random = new Random();

		// Kontinuerlig loop för att skicka temperaturdata.
		while (true)
		{
			// Skapar en ny simulerad väderprognos.
			var forecast = new WeatherForecast
			{
				Date = DateTime.Now,
				TemperatureC = random.Next(-20, 35),
				Summary = "Simulerad"
			};

			var jasonforcast = JsonConvert.SerializeObject(forecast);
			// Krypterar den genererade temperaturen med den definierade nyckeln.
			string encryptedTemperature = EncryptTemperature(jasonforcast, key, iv);

			// Skickar den krypterade temperaturen till SignalR-servern.
			try
			{
				await connection.SendAsync("SendTemperatureData", encryptedTemperature);
				// Väntar i 5 sekunder innan nästa iteration av loopen.
				await Task.Delay(5000);
			}
			catch (Exception ex)
			{
				// Om det finns något fel, skrivs det ut till konsolen.
				Console.WriteLine($"Error for: {ex.Message}");
			}
	
		}
	}

	// Funktion för att kryptera temperaturdata med AES-kryptering.
	static string EncryptTemperature(string temperature, byte[] key, byte[] iv)
	{

		using (Aes aes = Aes.Create())
		{
			aes.Key = key;
			aes.IV = iv;

			ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
				using (StreamWriter sw = new StreamWriter(cs))
				{
					sw.Write(temperature);
				}
				return Convert.ToBase64String(ms.ToArray());
			}
		}
	}
}
