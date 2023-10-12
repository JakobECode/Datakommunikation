using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Security.Cryptography;
using System.Text;
using TempGenerator;

class Program
{
	// En konstant nyckel genereras vid körning för att kryptera temperaturdata.
	private static readonly byte[] Key = GenerateRandomKey();

	// En konstant DeviceId genereras vid körning för att simulera olika enheter.
	private static readonly string DeviceId = $"Device-{new Random().Next(1000, 9999)}";

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

			// Krypterar den genererade temperaturen med den definierade nyckeln.
			string encryptedTemperature = EncryptTemperature(forecast.TemperatureC, Key);

			// Skickar den krypterade temperaturen till SignalR-servern.
			try
			{
				await connection.SendAsync("SendTemperatureData", DeviceId, encryptedTemperature);
			}
			catch (Exception ex)
			{
				// Om det finns något fel, skrivs det ut till konsolen.
				Console.WriteLine($"Error for {DeviceId}: {ex.Message}");
			}

			// Väntar i 5 sekunder innan nästa iteration av loopen.
			await Task.Delay(5000);
		}
	}

	// Funktion för att generera en slumpmässig krypteringsnyckel.
	public static byte[] GenerateRandomKey()
	{
		using (var randomNumberGenerator = new RNGCryptoServiceProvider())
		{
			var key = new byte[32];
			randomNumberGenerator.GetBytes(key);
			byte[] Key = {
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
	};
			return Key;	
		}
	}

	// Funktion för att kryptera temperaturdata med AES-kryptering.
	public static string EncryptTemperature(int temperature, byte[] key)
	{
		using (Aes aes = Aes.Create())
		{
			aes.Key = key;
			aes.GenerateIV();
			byte[] encryptedTemperature;

			using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
			{
				byte[] temperatureBytes = Encoding.UTF8.GetBytes(temperature.ToString());
				encryptedTemperature = encryptor.TransformFinalBlock(temperatureBytes, 0, temperatureBytes.Length);
			}

			// Kombinerar initialization vektorn (IV) med den krypterade datan.
			//byte[] result = new byte[aes.IV.Length + encryptedTemperature.Length];
			//Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
			//Buffer.BlockCopy(encryptedTemperature, 0, result, aes.IV.Length, encryptedTemperature.Length);

			return Convert.ToBase64String(encryptedTemperature);
		}
	}
}
