const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7045/TemperatureHub")
    .withAutomaticReconnect()
    .build();

    // Försöker starta SignalR-anslutningen.
connection.start()
    .then(function(){
        console.log("Successfully connected to the hub.");
    })
    .catch(function(){
        console.error("Error while connecting to the hub:", error.toString());
    });

// Definiera en händelse när ett meddelande mottas från servern.
connection.on("ReceiveTempData", function (forecast) {
   console.log(forecast)
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
