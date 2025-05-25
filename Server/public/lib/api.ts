export async function fetchPlayers() {
  const res = await fetch('http://localhost:3000/players');
  return res.json();
}

export async function fetchPositions() {
  const res = await fetch('http://localhost:3000/positions');
  const data = await res.json();
  const map: Record<string, any> = {};
  data.forEach(p => map[p.player_id] = p.position);
  return map;
}

export async function fetchUsers() {
  const res = await fetch('http://localhost:3000/users');
  return res.json();
}

export async function disconnectPlayer(playerId: string) {
  await fetch(`http://localhost:3000/player/${playerId}`, { method: 'DELETE' });
}
