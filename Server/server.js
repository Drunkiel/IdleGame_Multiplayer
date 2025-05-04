const express = require('express');
const cors = require('cors');
const { v4: uuidv4 } = require('uuid');

const app = express();
const port = 3000;

// Baza danych graczy (słownik przechowujący ID i status)
const players = {};

app.use(cors());
app.use(express.json());

// Endpoint do połączenia z serwerem i przypisania ID
app.post('/connect', (req, res) => {
    // Generowanie unikalnego ID dla nowego gracza
    const playerId = uuidv4();
    
    // Dodawanie gracza do bazy danych (obiekt)
    players[playerId] = {
        status: "connected",
        position: { x: 0, y: 0, z: 0 },
        lastSeen: Date.now()
    }; 

    res.json({
        message: "Succesfully connected to server.",
        player_id: playerId,
        status: "connected"
    });
});

app.post('/update_position/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const pos = req.body;

    if (players[playerId]) {
        players[playerId].lastSeen = Date.now();
        players[playerId].position = {
            x: pos.x,
            y: pos.y,
            z: pos.z
        };
        res.json({ message: "Position updated", player_id: playerId });
    } else {
        res.status(404).json({ error: "Player: " + playerId + " does not exist" });
    }
});

// Gracz potwierdza, że nadal żyje
app.post('/heartbeat/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    if (players[playerId]) {
        players[playerId].lastSeen = Date.now(); // aktualizuj czas ostatniego kontaktu
        res.json({ status: 'ok' });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

app.get('/positions', (req, res) => {
    const positions = Object.entries(players).map(([id, data]) => ({
        player_id: id,
        position: data.position
    }));
    res.json(positions);
});

// Endpoint do sprawdzenia statusu gracza
app.get('/status/:player_id', (req, res) => {
    const playerId = req.params.player_id;

    if (players[playerId]) {
        players[playerId].lastSeen = Date.now();
        res.json({
            player_id: playerId,
            status: players[playerId].status
        });
    } else {
        res.status(404).json({
            error: "Player: " + playerId + " does not exist"
        });        
    }
});

// Endpoint do zmiany statusu gracza
app.post('/update_status/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const newStatus = req.body.status;

    if (players[playerId]) {
        players[playerId].lastSeen = Date.now();
        players[playerId].status = newStatus;
        res.json({
            player_id: playerId,
            status: newStatus
        });
    } else {
        res.status(404).json({
            error: "Player: " + {playerId} + " does not exists"
        });
    }
});

// Endpoint do pobrania listy graczy
app.get('/players', (req, res) => {
    const playerList = Object.keys(players).map(playerId => ({
        player_id: playerId,
        status: players[playerId].status
    }));

    res.json(playerList);
});

// Usunięcie gracza z serwera
app.delete('/player/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    if (players[playerId]) {
        delete players[playerId];
        res.json({ message: `Player ${playerId} removed.` });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

// Startowanie serwera
app.listen(port, () => {
    console.log(`Server runs on port: ${port}`);
});

setInterval(() => {
    const now = Date.now();
    const timeout = 300000; // 5 minut braku sygnału

    for (const playerId in players) {
        if (now - players[playerId].lastSeen > timeout) {
            console.log(`Removing inactive player: ${playerId}`);
            delete players[playerId];
        }
    }
}, 10000);

