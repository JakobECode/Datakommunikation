using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Text;

public class TemperatureHub : Hub
{
	// En logger används för att spåra och logga händelser eller fel.
	private readonly ILogger<TemperatureHub> _logger;
	private static readonly byte[] Key = {
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
	};

	// Konstruktorn tar en ILogger som en beroende (injicerad av ASP.NET Core).
	public TemperatureHub(ILogger<TemperatureHub> logger)
	{
		_logger = logger;
	}

	// En metod för att hämta krypteringsnyckeln. 
	private byte[] GetEncryptionKey()
	{
		// Just nu returneras en hårdkodad nyckel, men denna metod kan senare anpassas 
		// för att hämta nyckeln från en säker källa som en konfigurationslagring.
		return new byte[]
		{
			// ... (nyckelvärden) ...
		};
	}

	// Metoden som tar emot krypterad temperaturdata från en klient, dekrypterar den och 
	// sänder den till alla anslutna klienter.
	public async Task SendTemperatureData(string deviceId, string encryptedTemperature)
	{
		// Validera inmatningen för att försäkra att den inte är tom eller ogiltig.
		if (string.IsNullOrWhiteSpace(deviceId) || string.IsNullOrWhiteSpace(encryptedTemperature))
		{
			_logger.LogWarning("Invalid input received in SendTemperatureData for device {DeviceId}.", deviceId);
			return;
		}

		try
		{
			// Dekryptera temperaturen med den givna nyckeln.
			//double temperature = DecryptTemperature(encryptedTemperature, Key);

			// Skicka den dekrypterade temperaturen till alla anslutna klienter.
			await Clients.All.SendAsync("PublishToClient", deviceId, encryptedTemperature);
		}
		catch (Exception ex)
		{
			// Om något går fel, logga felet och informera klienten.
			_logger.LogError(ex, "Error occurred while sending temperature data for device {DeviceId}.", deviceId);
		}
	}

	// En hjälpmetod som dekrypterar den krypterade temperaturen.
	public double DecryptTemperature(string encryptedTemperature, byte[] key)
	{
		try
		{
			// Skapa en AES-instans för dekryptering.
			using (Aes aes = Aes.Create())
			{
				aes.Key = key;

				// Dela upp den mottagna datasträngen i IV (Initialization Vector) och den krypterade delen.
				byte[] fullBuffer = Convert.FromBase64String(encryptedTemperature);
				byte[] iv = new byte[16];
				byte[] encrypted = new byte[fullBuffer.Length - iv.Length];

				Buffer.BlockCopy(fullBuffer, 0, iv, 0, iv.Length);
				Buffer.BlockCopy(fullBuffer, iv.Length, encrypted, 0, encrypted.Length);

				aes.IV = iv;

				// Utför själva dekrypteringsprocessen.
				using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
				{
					byte[] decryptedTemperatureBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
					return Convert.ToDouble(Encoding.UTF8.GetString(decryptedTemperatureBytes));
				}
			}
		}
		catch (Exception ex)
		{
			// Om något går fel med dekrypteringsprocessen, logga felet och kasta ett nytt undantag.
			_logger.LogError(ex, "Error occurred while decrypting temperature data.");
			throw new InvalidOperationException("Failed to decrypt the temperature data.");
		}
	}
}







