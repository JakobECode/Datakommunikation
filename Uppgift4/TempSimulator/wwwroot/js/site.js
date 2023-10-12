// Skapar en ny SignalR-anslutning med den angivna URL:en.
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/TemperatureHub")
    .configureLogging(signalR.LogLevel.Information)  // Lägger till loggningsinformation, kan vara användbart för felsökning.
    .build();

// Definiera en händelse när ett meddelande mottas från servern.
connection.on("PublishToClient", function (forecast) {
    if (forecast && typeof forecast === 'object') { // Validera mottagen data.
        // Visar mottagen temperaturdata i webbläsarens konsol.
        console.log(forecast);

        // Här kan du också lägga till logik för att visa datan på ditt webbgränssnitt, t.ex. uppdatera ett HTML-element.
    } else {
        console.error("Received invalid forecast data from the server.");
    }
});

// Försöker starta SignalR-anslutningen.
connection.start()
    .then(() => {
        console.log("Successfully connected to the hub.");
    })
    .catch((error) => {
        console.error("Error while connecting to the hub:", error);
    });

// Definiera en händelse när anslutningen avbryts.
connection.onclose(async () => {
    console.warn("Connection to the hub was closed. Trying to reconnect...");
    try {
        await connection.start();
        console.log("Reconnected successfully.");
    } catch (err) {
        console.error("Error while trying to reconnect:", err);
        setTimeout(() => connection.start(), 5000); // Försök återansluta var 5:e sekund.
    }
});
