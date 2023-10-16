using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Text.Json;
using TempSignalRServer.Models;

public class TemperatureHub : Hub
{
	static byte[] key = {
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
};

	static byte[] iv = new byte[16]; // Initialization Vector. You can generate and store this if needed. 
	

	// Metoden som tar emot krypterad temperaturdata från en klient, dekrypterar den och 
	// sänder den till alla anslutna klienter.
	public async Task ReceiveTemperatureData(string encryptedTemperature)
	{
		try
		{
			string decryptedData = Decrypt(encryptedTemperature, key, iv);

			WeatherForecast forecast = JsonSerializer.Deserialize<WeatherForecast>(decryptedData)!;

			Console.WriteLine($"Received temperature forecast - Date: {forecast.Date}, Temperature: {forecast.TemperatureC}, Summary: {forecast.Summary} Jakob testar");

			await Clients.All.SendAsync("ReceiveTempData", forecast);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	// En hjälpmetod som dekrypterar den krypterade temperaturen.
	private string Decrypt(string encryptedText, byte[] key, byte[] iv)
	{
		try
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = iv;

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
				using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
				using (StreamReader sr = new StreamReader(cs))
				{
					return (sr.ReadToEnd());
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return (ex.Message);
		}

	}
}







