using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using TempGenerator;

class Program
{
    // En statisk variabel som håller anslutningen till SignalR-hubben.
    public static HubConnection? connection;

    // Hårdkodade krypteringsnyckel och initialiseringsvektor (IV).
    private static byte[] EncryptionKey = new byte[]
    {
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0,
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
    };
    private static byte[] iv = new byte[16];

    static async Task Main(string[] args)
    {
        // Skapar en instans av HubHandler som hanterar kommunikation med SignalR-hubben.
        HubHandler handler = new HubHandler();

        // Startar och initialiserar anslutningen till hubben.
        await handler.InitialyzeAsync();
        // Sänder simulerad temperaturdata till hubben kontinuerligt.
        await handler.SendTempAsync();
    }

    public class HubHandler
    {
        // HubConnection representerar anslutningen till SignalR-hubben.
        HubConnection hubConn = null!;

        public HubHandler()
        {
            // Bygger anslutningen till SignalR-hubben med en specifik URL och använder WebSockets som transportmetod.
            hubConn = new HubConnectionBuilder().WithUrl("https://localhost:7045/TemperatureHub", HttpTransportType.WebSockets).Build();
        }

        public async Task InitialyzeAsync()
        {
            // Startar anslutningen till hubben.
            await hubConn.StartAsync();
        }

        public async Task SendTempAsync()
        {
            // Loopar oändligt för att regelbundet skicka temperaturdata.
            while (true)
            {
                var random = new Random();
                // Skapar ett simulerat väderprognosobjekt.
                WeatherForecast forecast = new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = random.Next(-20, 35), // Genererar en slumpmässig temperatur mellan -20 och 35 grader.
                    Summary = "simulated"
                };

                // Serialiserar väderprognosobjektet till en JSON-sträng.
                string json = JsonConvert.SerializeObject(forecast);

                // Krypterar JSON-strängen med hjälp av den givna nyckeln och IV:n.
                string encryptedData = Encrypt(json, EncryptionKey, iv);

                // Sänder den krypterade datan till SignalR-hubben.
                await hubConn.SendAsync("ReceiveTemperturData", encryptedData);

                // Väntar i 5 sekunder innan nästa sändning.
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        // En hjälpmetod för att kryptera strängar med Aes-kryptering.
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
                        // Skriver in temperaturen i krypteringsströmmen.
                        sw.Write(temperature);
                    }
                    // Returnerar den krypterade datan som en base64-sträng.
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
