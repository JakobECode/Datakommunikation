using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using TempGenerator;

class Program
{
    public static HubConnection? connection;

    private static byte[] EncryptionKey = new byte[]
    {
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
    };
    private static byte[] iv = new byte[16];

    static async Task Main(string[] args)
    {
        HubHandler handler = new HubHandler();

        await handler.InitialyzeAsync();
        await handler.SendTempAsync();
    }

    public class HubHandler
    {
        HubConnection hubConn = null!;

        public HubHandler()
        {
            hubConn = new HubConnectionBuilder().WithUrl("https://localhost:7045/TemperatureHub", HttpTransportType.WebSockets).Build();
        }

        public async Task InitialyzeAsync()
        {
            await hubConn.StartAsync();
        }

        public async Task SendTempAsync()
        {
            while (true)
            {
                var random = new Random();
                WeatherForecast forecast = new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = random.Next(-20, 35),
                    Summary = "simulated"
                };

                //Serialisera objektet forecast till JSON
                string json = JsonConvert.SerializeObject(forecast);

                //Kryptera JSON-strängen
                string encryptedData = Encrypt(json, EncryptionKey, iv);

                await hubConn.SendAsync("ReceiveTemperturData", encryptedData);

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        private static string Encrypt(string temperature, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

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


}
// lägg till kryptering här. 