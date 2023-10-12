using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public static class CryptoHelper
{
	// Denna konstant nyckel används för AES-kryptering. 
	// Det är bra att notera att i en riktig produktionssituation skulle nyckeln lagras på ett säkert ställe snarare än att vara hårdkodad.
	private static readonly byte[] Key = {
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
		0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
	};

	// EncryptData metoden tar ett dataobjekt, serialiserar det till en sträng med JSON och krypterar sedan strängen med AES.
	public static string EncryptData<T>(T data)
	{
		// Konvertera det generiska dataobjektet till en JSON-sträng.
		string plaintext = JsonConvert.SerializeObject(data);

		using (Aes aes = Aes.Create())
		{
			aes.Key = Key;
			byte[] encryptedData;

			using (MemoryStream memoryStream = new MemoryStream())
			{
				// Skriv IV till början av strömmen.
				memoryStream.Write(aes.IV, 0, aes.IV.Length);

				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
				using (StreamWriter writer = new StreamWriter(cryptoStream))
				{
					writer.Write(plaintext);
				}
				encryptedData = memoryStream.ToArray();
			}

			// Returnera den krypterade datan som en Base64-sträng.
			return Convert.ToBase64String(encryptedData);
		}
	}

	// DecryptData metoden tar en krypterad Base64-sträng, dekrypterar den och konverterar sedan tillbaka till ett generiskt objekt.
	public static T DecryptData<T>(string encryptedData)
	{
		byte[] cipherTextBytes = Convert.FromBase64String(encryptedData);

		using (Aes aes = Aes.Create())
		{
			aes.Key = Key;

			byte[] iv = new byte[aes.IV.Length];
			Array.Copy(cipherTextBytes, iv, iv.Length);
			aes.IV = iv;

			using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
			{
				memoryStream.Seek(iv.Length, SeekOrigin.Begin);
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
				using (StreamReader reader = new StreamReader(cryptoStream))
				{
					string decryptedText = reader.ReadToEnd();
					// Konvertera den dekrypterade strängen tillbaka till ett generiskt objekt.
					return JsonConvert.DeserializeObject<T>(decryptedText);
				}
			}
		}
	}
}
