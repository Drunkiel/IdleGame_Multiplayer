'use client';

import React, { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';

type Player = {
  player_id: string;
  username: string;
  scene: string;
  [key: string]: any;
};

const PlayerDetailsPage: React.FC = () => {
  const { playerId } = useParams() as { playerId: string };
  const [player, setPlayer] = useState<Player | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();

  useEffect(() => {
    async function fetchPlayer() {
      try {
        setLoading(true);
        const res = await fetch(`http://localhost:3000/player/${playerId}`);
        if (!res.ok) {
          throw new Error(`Błąd pobierania danych gracza: ${res.status}`);
        }
        const data = await res.json();
        setPlayer(data);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    fetchPlayer();
  }, [playerId]);

  if (loading) return <p>Ładowanie danych gracza...</p>;
  if (error) return <p style={{ color: 'red' }}>Błąd: {error}</p>;
  if (!player) return <p>Nie znaleziono gracza.</p>;

  return (
    <div style={{ padding: '20px' }}>
      <button onClick={() => router.back()} style={{ marginBottom: '20px' }}>
        ← Powrót
      </button>
      <h1>Szczegóły gracza: {player.username}</h1>
      <pre
        style={{
          backgroundColor: '#f4f4f4',
          padding: '15px',
          borderRadius: '6px',
          overflowX: 'auto',
          maxHeight: '70vh',
        }}
      >
        {JSON.stringify(player, null, 2)}
      </pre>
    </div>
  );
};

export default PlayerDetailsPage;
