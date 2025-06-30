const express = require('express');
const cors = require('cors');
const { v4: uuidv4 } = require('uuid');
const fs = require('fs');
const path = require('path');
const DATA_FILE = path.join(__dirname, 'players.json');

const app = express();
const port = 3000;

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// In-memory database of users
const testDB = {};
if (fs.existsSync(DATA_FILE)) {
    try {
        const rawData = fs.readFileSync(DATA_FILE);
        const savedData = JSON.parse(rawData);
        Object.assign(testDB, savedData);
        console.log('Loaded player data from file.');
    } catch (err) {
        console.error('Failed to load player data:', err);
    }
}

function savePlayerDataToFile() {
    fs.writeFile(DATA_FILE, JSON.stringify(testDB, null, 2), (err) => {
        if (err) {
            console.error('Failed to save player data:', err);
        } else {
            console.log('Player data saved to players.json');
        }
    });
}

// Save testDB to file every 5 minutes
setInterval(() => {
    savePlayerDataToFile();
}, 5 * 60 * 1000); 

//Actual in game players
const users = {};

// === AUTH ===
app.post('/login', (req, res) => {
    const { username, password } = req.body;
    const user = testDB[username];

    //Check if player is logged in
    if (user && users[user.playerId] === user) {
        return res.status(403).json({ error: "Player already logged in" });
    }

    //Check if password and username are correct
    if (user && user.password === password) {
        users[user.playerId] = user;

        const data = user.playerData;

        res.json({
            player_id: user.playerId,
            scene: data.scene,
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
            luckPoints: data.luckPoints,
            inventory: data.inventory,
            activeQuests: data.activeQuests,
            completedQuests: data.completedQuests
        });
    } else {
        res.status(401).json({ error: "Invalid username or password" });
    }
});

app.post('/register', (req, res) => {
    const { username, password, heroClass } = req.body;

    // REGEX - Walidacja
    const usernameRegex = /^[a-zA-Z0-9_]{5,20}$/; // bez spacji, tylko litery, cyfry, podkreślenia
    const passwordRegex = /^(?=.*[A-Z])(?=.*\d).{8,}$/; // min. 8 znaków, 1 wielka litera, 1 cyfra

    if (!username || !password || !heroClass) {
        return res.status(400).json({ error: 'Missing fields' });
    }

    const trimmedUsername = username.trim();

    // // Sprawdzenie nazwy użytkownika
    // if (!usernameRegex.test(trimmedUsername)) {
    //     return res.status(400).json({ error: 'Invalid username. Only letters, numbers, and underscores are allowed.' });
    // }

    // // Sprawdzenie hasła
    // if (!passwordRegex.test(password)) {
    //     return res.status(400).json({ error: 'Invalid password. Must be at least 8 characters, include one uppercase letter and one number.' });
    // }

    //Check if user exist
    if (testDB[trimmedUsername]) {
        return res.status(400).json({ error: 'Username already exists' });
    }

    const playerId = uuidv4();
    testDB[trimmedUsername] = {
        password,
        playerId,
        playerData: {
            username: trimmedUsername,
            status: "Disconnected",
            position: { x: 0, y: 0, z: 0 },
            lastSeen: Date.now(),
            scene: "Nothingness",
            heroClass: heroClass,
            currentLevel: 1,
            expPoints: 0,
            goldCoins: 0,
            strengthPoints: 0,
            dexterityPoints: 0,
            intelligencePoints: 0,
            durablityPoints: 0,
            luckPoints: 0,
            armorPoints: 0,
            inventory: Array.from({ length: 26 }, (_, i) => ({
                slotId: i,
                itemID: null
            })),
            activeQuests: [],
            completedQuests: []
        }
    };

    res.json({ message: 'Registered successfully', player_id: playerId });
});

// === PLAYER DATA ===
app.get('/player/:player_id', (req, res) => {
    const playerId = req.params.player_id;

    const user = Object.values(testDB).find(u => u.playerId === playerId);

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

//==INVENTORY==
app.get('/inventory/:player_id', (req, res) => {
  const playerId = req.params.player_id;
  const user = Object.values(users).find(u => u.playerId === playerId);

  if (!user) {
    return res.status(404).json({ error: 'Player not found' });
  }

  const inventoryData = user.playerData.inventory.map(slot => ({
    slotID: slot.slotId,
    itemID: slot.itemID
      ? {
          _itemData: {
            ID: slot.itemID._itemData.ID,
            baseStat: {
              value: slot.itemID._itemData.baseStat?.value || 0
            },
            additionalAttributeStats: slot.itemID._itemData.additionalAttributeStats || []
          }
        }
      : null
  }));

  res.json(inventoryData);
});

app.post('/inventory/:player_id', (req, res) => {
  const playerId = req.params.player_id;
  const updatedPayload = req.body;

  const updatedSlots = updatedPayload?.Items;
  if (!Array.isArray(updatedSlots)) {
    return res.status(400).json({ error: 'Invalid inventory data format' });
  }

  const user = Object.values(users).find(u => u.playerId === playerId);
  if (!user) {
    return res.status(404).json({ error: 'Player not found' });
  }

  const inventory = user.playerData.inventory;

  updatedSlots.forEach(slot => {
    const { slotID, itemID } = slot;

    if (typeof slotID !== 'number' || slotID < 0 || slotID >= inventory.length) {
      console.warn(`Ignored invalid slotID: ${slotID}`);
      return;
    }

    // Zwaliduj itemID._itemData.ID
    if (itemID?._itemData?.ID === -1) {
      // Ustawiamy na null, czyli slot pusty
      inventory[slotID] = {
        slotID,
        itemID: null
      };
    } else {
      inventory[slotID] = {
        slotID,
        itemID: {
          _itemData: {
            ID: itemID._itemData.ID,
            baseStat: {
              value: itemID._itemData.baseStat?.value || 0
            },
            additionalAttributeStats: itemID._itemData.additionalAttributeStats || []
          }
        }
      };
    }
  });

  res.json({ message: 'Inventory updated successfully' });
});

app.post('/update_quests/:player_id', (req, res) => {
  const playerId = req.params.player_id;
  const { activeQuests, completedQuests } = req.body;

  const user = Object.values(users).find(u => u.playerId === playerId);
  if (!user) {
    return res.status(404).json({ error: 'Player not found' });
  }

  if (Array.isArray(activeQuests)) {
    user.playerData.activeQuests = activeQuests;
  }

  if (Array.isArray(completedQuests)) {
    user.playerData.completedQuests = completedQuests;
  }

  user.playerData.lastSeen = Date.now();

  res.json({
    message: 'Quests updated',
    player_id: playerId,
    activeQuests: user.playerData.activeQuests,
    completedQuests: user.playerData.completedQuests
  });
});

app.get('/quests/:player_id', (req, res) => {
  const playerId = req.params.player_id;

  const user = Object.values(users).find(u => u.playerId === playerId);
  if (!user) {
    return res.status(404).json({ error: 'Player not found' });
  }

  res.json({
    activeQuests: user.playerData.activeQuests || [],
    completedQuests: user.playerData.completedQuests || []
  });
});

app.get('/users', (req, res) => {
    const usersArray = Object.entries(testDB).map(([username, data]) => ({
        username,
        password: data.password,
        playerId: data.playerId
    }));
    res.json(usersArray);
});

app.post('/disconnect_all', (req, res) => {
    const now = Date.now();
    const disconnectedPlayers = [];

    for (const username in users) {
        const player = users[username];
        player.playerData.status = "Disconnected";
        player.playerData.lastSeen = now;
        disconnectedPlayers.push({
            player_id: player.playerId,
            username,
            status: "Disconnected"
        });
    }

    // Wyczyszczenie listy aktywnych graczy
    Object.keys(users).forEach(username => {
        delete users[username];
    });

    console.log(`Disconnected all players at ${new Date().toISOString()}`);
    res.json({ message: "All players have been disconnected", players: disconnectedPlayers });
});

// Cleanup inactive players
setInterval(() => {
    const now = Date.now();
    const timeout = 5 * 60 * 1000; // 5 minutes

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
