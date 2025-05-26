'use client';

import React from 'react';

type Player = {
  player_id: string;
  username: string;
  scene: string;
  status: string;
  [key: string]: any;
};

type Props = {
  players: Player[];
  onDisconnect: (playerId: string) => void;
};

import { useRouter } from 'next/navigation';

const CurrentUsersPanel: React.FC<Props> = ({ players, onDisconnect }) => {
  const router = useRouter();

  return (
    <>
      <h2>Aktualnie zalogowani gracze: {players.length}</h2>
      <div className="players-grid">
        {players.map((player) => (
          <div key={player.player_id} className="player-card">
            <div className="player-info">
              <p><strong>ID:</strong> <span className="ellipsis">{player.player_id}</span></p>
              <p><strong>Username:</strong> <span className="ellipsis">{player.username}</span></p>
              <p><strong>Scene:</strong> <span className="ellipsis">{player.scene}</span></p>
              <p><strong>Status:</strong> <span className="ellipsis">{player.status}</span></p>
            </div>
            <div className="player-actions">
              <button className="btn-disconnect" onClick={() => onDisconnect(player.player_id)}>
                Rozłącz
              </button>
              <button
                className="btn-details"
                onClick={() => router.push(`/player/${player.player_id}`)}
              >
                Szczegóły
              </button>
            </div>
          </div>
        ))}
      </div>

      <style jsx>{`
        .players-grid {
          display: grid;
          grid-template-columns: repeat(5, 1fr);
          gap: 16px;
        }

        .player-card {
          background: white;
          border: 1px solid #ccc;
          border-radius: 8px;
          padding: 16px;
          height: 175px;
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          box-shadow: 0 2px 6px rgba(0,0,0,0.1);
        }

        .player-info p {
          margin: 6px 0;
          overflow: hidden;
          white-space: nowrap;
          text-overflow: ellipsis;
        }

        .ellipsis {
          display: inline-block;
          max-width: 100%;
          vertical-align: bottom;
          overflow: hidden;
          white-space: nowrap;
          text-overflow: ellipsis;
        }

        .player-actions {
          display: flex;
          justify-content: space-between;
          margin-top: 12px;
        }

        .btn-disconnect, .btn-details {
          padding: 8px 12px;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-weight: 600;
          transition: background-color 0.2s ease;
        }

        .btn-disconnect {
          background-color: #e74c3c;
          color: white;
        }

        .btn-disconnect:hover {
          background-color: #c0392b;
        }

        .btn-details {
          background-color: #3498db;
          color: white;
        }

        .btn-details:hover {
          background-color: #217dbb;
        }

        /* Responsywność: na mniejszych ekranach jedna lub dwie kolumny */
        @media (max-width: 900px) {
          .players-grid {
            grid-template-columns: repeat(2, 1fr);
          }
        }
        @media (max-width: 600px) {
          .players-grid {
            grid-template-columns: 1fr;
          }
        }
      `}</style>
    </>
  );
};

export default CurrentUsersPanel;
