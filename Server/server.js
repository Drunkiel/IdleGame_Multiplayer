const express = require('express');
const cors = require('cors');
const { v4: uuidv4 } = require('uuid');

const app = express();
const port = 3000;

const players = {};

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

//Connection to server
app.post('/connect', (req, res) => {
    const playerId = uuidv4();
    
    players[playerId] = {
        status: "connected",
        position: { x: 0, y: 0, z: 0 },
        lastSeen: Date.now(),
        scene: "unknown"
    }; 

    res.json({
        message: "Succesfully connected to server.",
        player_id: playerId,
        status: "connected",
        lastSeen: Date.now(),
        scene: "unknown"
    });
});

app.post('/update_position/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const { x, y, z } = req.body;

    if (players[playerId]) {
        players[playerId].position = { x, y, z };
        players[playerId].lastSeen = Date.now();
        res.json({ success: true });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

//Check if player is still active
app.post('/heartbeat/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    if (players[playerId]) {
        players[playerId].lastSeen = Date.now(); 
        res.json({ status: 'ok' });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

app.get('/positions', (req, res) => {
    const requesterId = req.query.player_id;

    if (!requesterId) {
        const all = Object.entries(players).map(([id, data]) => ({
            player_id: id,
            position: data.position,
            scene: data.scene
        }));
        return res.json(all);
    }

    const requester = players[requesterId];
    if (!requester) {
        return res.status(404).json({ error: 'Invalid requester' });
    }

    const sameScenePlayers = Object.entries(players)
        .filter(([id, data]) => id !== requesterId && data.scene === requester.scene)
        .map(([id, data]) => ({
            player_id: id,
            position: data.position,
            scene: data.scene
        }));

    res.json(sameScenePlayers);
});

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

app.get('/scene/:player_id', (req, res) => {
    const playerId = req.params.player_id;

    if (players[playerId]) {
        players[playerId].lastSeen = Date.now();
        res.json({
            player_id: playerId,
            scene: players[playerId].scene
        });
    } else {
        res.status(404).json({
            error: "Player: " + playerId + " does not exist"
        });        
    }
});

app.post('/update_scene/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const newScene = req.body.scene;

    if (players[playerId]) {
        players[playerId].lastSeen = Date.now();
        players[playerId].scene = newScene;
        res.json({
            player_id: playerId,
            scene: newScene
        });
    } else {
        res.status(404).json({
            error: "Player: " + {playerId} + " does not exists"
        });
    }
});

//Get ALL players
app.get('/players', (req, res) => {
    const playerList = Object.keys(players).map(playerId => ({
        player_id: playerId,
        status: players[playerId].status,
        scene: players[playerId].scene
    }));

    res.json(playerList);
});

//Delete player from server
app.delete('/player/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    if (players[playerId]) {
        delete players[playerId];
        res.json({ message: `Player ${playerId} removed.` });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

//Start server
app.listen(port, () => {
    console.log(`Server runs on port: ${port}`);
});

setInterval(() => {
    const now = Date.now();
    const timeout = 300000;

    for (const playerId in players) {
        if (now - players[playerId].lastSeen > timeout) {
            console.log(`Removing inactive player: ${playerId}`);
            delete players[playerId];
        }
    }
}, 10000);

