'use client';

import React from 'react';

type Player = {
  player_id: string;
  username?: string;
  status?: string;
  [key: string]: any;
};

type Position = {
  x: number;
  y: number;
  z: number;
};

type Props = {
  players: Player[];
  positions: Record<string, Position>;
  onDisconnect: (playerId: string) => void;
};

const PlayerTable: React.FC<Props> = ({ players, positions, onDisconnect }) => {
  if (players.length === 0) {
    return <p>Brak połączonych graczy.</p>;
  }

  const keys = Object.keys(players[0]).filter((key) => key !== 'position');
  if (!keys.includes('position')) {
    keys.splice(2, 0, 'position'); // Dodaj "position" jako trzecią kolumnę
  }

  return (
    <table>
      <thead>
        <tr>
          {keys.map((key) => (
            <th key={key}>{key}</th>
          ))}
          <th>Akcja</th>
        </tr>
      </thead>
      <tbody>
        {players.map((player) => (
          <tr key={player.player_id}>
            {keys.map((key) => {
              let content: React.ReactNode = '';

              if (key === 'position') {
                const pos = positions[player.player_id];
                content = pos
                  ? `(${pos.x.toFixed(1)}, ${pos.y.toFixed(1)}, ${pos.z.toFixed(1)})`
                  : 'brak danych';
              } else if (key === 'inventory' && typeof player[key] === 'object') {
                const inv = player[key];
                content = `slot: ${inv.slotId}, item: ${inv.itemID}`;
              } else {
                content = player[key];
              }

              return (
                <td key={key} className={key === 'status' ? 'status' : ''}>
                  {content}
                </td>
              );
            })}
            <td>
              <button
                className="btn-disconnect"
                onClick={() => onDisconnect(player.player_id)}
              >
                Rozłącz
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default PlayerTable;
