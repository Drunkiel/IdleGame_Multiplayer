const express = require('express');
const cors = require('cors');
const { v4: uuidv4 } = require('uuid');

const app = express();
const port = 3000;

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// In-memory database of users
const testDB = {};
testDB["a"] = {
    password: "a",
    playerId: uuidv4(),
    playerData: {
        username: "Denis",
        status: "Disconnected",
        position: { x: 0, y: 0, z: 0 },
        lastSeen: Date.now(),
        scene: "unknown",
        heroClass: 1,
        currentLevel: 69,
        expPoints: 1,
        goldCoins: 1,
        strengthPoints: 1,
        dexterityPoints: 1,
        intelligencePoints: 1,
        durablityPoints: 1,
        luckPoints: 1,
    }
};
//Actual in game players
const users = {};

// === AUTH ===
app.post('/login', (req, res) => {
    const { username, password } = req.body;
    const user = testDB[username];

    //Check if player is logged in
    if (users[user.playerId] === user) {
        return res.status(403).json({ error: "Player already logged in" });
    }

    //Check if password and username are correct
    if (user && user.password === password) {
        users[user.playerId] = user;

        const data = user.playerData;

        res.json({
            player_id: user.playerId,
            status: data.status,
            heroClass: data.heroClass,
            username: data.username,
            currentLevel: data.currentLevel,
            expPoints: data.expPoints,
            goldCoins: data.goldCoins,
            strengthPoints: data.strengthPoints,
            dexterityPoints: data.dexterityPoints,
            intelligencePoints: data.intelligencePoints,
            durablityPoints: data.durablityPoints,
            luckPoints: data.luckPoints
        });
    } else {
        res.status(401).json({ error: "Invalid username or password" });
    }
});

app.post('/register', (req, res) => {
    const { username, password, heroClass } = req.body;

    if (testDB[username]) {
        return res.status(400).json({ error: 'Username already exists' });
    }

    const playerId = uuidv4();
    testDB[username] = {
        password,
        playerId,
        playerData: {
            username,
            status: "Disconnected",
            position: { x: 0, y: 0, z: 0 },
            lastSeen: Date.now(),
            scene: "unknown",
            heroClass: heroClass,
            currentLevel: 1,
            expPoints: 0,
            goldCoins: 0,
            strengthPoints: 0,
            dexterityPoints: 0,
            intelligencePoints: 0,
            durablityPoints: 0,
            luckPoints: 0,
            armorPoints: 0
        }
    };

    res.json({ message: 'Registered successfully', player_id: playerId });
});

// === PLAYER DATA ===
app.get('/player/:player_id', (req, res) => {
    const playerId = req.params.player_id;

    const user = Object.values(users).find(u => u.playerId === playerId);

    if (user) {
        user.playerData.lastSeen = Date.now();
        res.json(user.playerData);
    } else {
        res.status(404).json({});
    }
});

app.get('/players', (req, res) => {
    const playerList = Object.values(users).map(u => ({
        player_id: u.playerId,
        ...u.playerData
    }));

    res.json(playerList);
});

app.post('/update_stats/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const stats = req.body;

    const user = Object.values(users).find(u => u.playerId === playerId);
    if (!user) {
        return res.status(404).json({ error: "Player not found" });
    }

    const allowedFields = [
        'heroClass', 'currentLevel', 'expPoints', 'goldCoins',
        'strengthPoints', 'dexterityPoints', 'intelligencePoints',
        'durablityPoints', 'luckPoints', 'armorPoints'
    ];

    allowedFields.forEach(field => {
        if (stats[field] !== undefined) {
            user.playerData[field] = stats[field];
        }
    });

    user.playerData.lastSeen = Date.now();

    res.json({
        message: 'Player stats updated',
        player_id: playerId,
        updatedStats: stats
    });
});

app.post('/update_position/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const { x, y, z } = req.body;

    const user = Object.values(users).find(u => u.playerId === playerId);

    if (user) {
        user.playerData.position = { x, y, z };
        user.playerData.lastSeen = Date.now();
        res.json({ success: true });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

app.post('/heartbeat/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const user = Object.values(users).find(u => u.playerId === playerId);

    if (user) {
        user.playerData.lastSeen = Date.now();
        res.json({ status: 'ok' });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

app.get('/positions', (req, res) => {
    const requesterId = req.query.player_id;

    if (!requesterId) {
        const all = Object.values(users).map(u => ({
            player_id: u.playerId,
            username: u.playerData.username,
            position: u.playerData.position,
            scene: u.playerData.scene
        }));
        return res.json(all);
    }

    const requester = Object.values(users).find(u => u.playerId === requesterId);
    if (!requester) {
        return res.status(404).json({ error: 'Invalid requester' });
    }

    const sameScenePlayers = Object.values(users)
        .filter(u => u.playerId !== requesterId && u.playerData.scene === requester.playerData.scene)
        .map(u => ({
            player_id: u.playerId,
            position: u.playerData.position,
            scene: u.playerData.scene
        }));

    res.json(sameScenePlayers);
});

app.post('/update_status/:player_id', (req, res) => {
    const playerId = req.params.player_id;
    const newStatus = req.body.status;

    const user = Object.values(users).find(u => u.playerId === playerId);
    if (user) {
        user.playerData.lastSeen = Date.now();
        user.playerData.status = newStatus;
        res.json({
            player_id: playerId,
            status: newStatus
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

    const user = Object.values(users).find(u => u.playerId === playerId);
    if (user) {
        user.playerData.lastSeen = Date.now();
        user.playerData.scene = newScene;
        res.json({
            player_id: playerId,
            scene: newScene
        });
    } else {
        res.status(404).json({
            error: "Player: " + playerId + " does not exist"
        });
    }
});

app.delete('/player/:player_id', (req, res) => {
    const playerId = req.params.player_id;

    const entry = Object.entries(users).find(([_, u]) => u.playerId === playerId);
    if (entry) {
        const [username] = entry;
        delete users[username];
        res.json({ message: `Player ${playerId} removed.` });
    } else {
        res.status(404).json({ error: 'Player not found' });
    }
});

app.get('/users', (req, res) => {
    const usersArray = Object.entries(testDB).map(([username, data]) => ({
        username,
        password: data.password,
        playerId: data.playerId
    }));
    res.json(usersArray);
});

// Cleanup inactive players
setInterval(() => {
    const now = Date.now();
    const timeout = 300000; // 5 minutes

    for (const username in users) {
        const player = users[username];
        if (now - player.playerData.lastSeen > timeout) {
            player.playerData.status = "Disconnected";
            console.log(`Removing inactive player: ${player.playerId}`);
            delete users[username];
        }
    }
}, 10000);

// Start server
app.listen(port, () => {
    console.log(`Server runs on port: ${port}`);
});
