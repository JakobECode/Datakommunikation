using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Security.Cryptography;
using TempServer.Models;

public class TemperatureHub : Hub
{
    private static readonly byte[] EncryptionKey = new byte[]
    {
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
    };
    private static readonly byte[] IV = new byte[16];

    public async Task ReceiveTemperturData(string encryptedData)
    {
        try
        {
            string decryptedData = Decrypt(encryptedData, EncryptionKey, IV);

            WeatherForecast forecast = JsonSerializer.Deserialize<WeatherForecast>(decryptedData)!;

            Console.WriteLine($"Received temperature forecast - Date: {forecast.Date}, Temperature: {forecast.TemperatureC}, Summary: {forecast.Summary} Jakob testar");

            await Clients.All.SendAsync("ReceiveTemperturData", forecast);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

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

