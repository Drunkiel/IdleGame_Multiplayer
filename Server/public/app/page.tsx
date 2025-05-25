'use client';

import React, { useEffect, useState } from 'react';
import RegisteredUsersPanel from '../components/RegisteredUsersPanel';
import CurrentUsersPanel from '../components/CurrentUsersPanel';

type User = {
  playerId: string;
  username: string;
  password: string;
};

type Player = {
  player_id: string;
  username: string;
  scene: string;
  [key: string]: any;
};

const HomePanel: React.FC<{ usersCount: number; playersCount: number; onRestart: () => void }> = ({
  usersCount,
  playersCount,
  onRestart,
}) => (
  <div className="home-container">
    <h2>Panel główny</h2>
    <div className="cards">
      <div className="card">
        <h3>Zarejestrowani gracze</h3>
        <p>{usersCount}</p>
      </div>
      <div className="card">
        <h3>Aktualnie zalogowani gracze</h3>
        <p>{playersCount}</p>
      </div>
    </div>
    <div className="restart-container">
      <button onClick={onRestart} className="btn-restart">
        Restart serwera
      </button>
    </div>

    <style jsx>{`
      .home-container {
        max-width: 600px;
        margin: 0 auto;
        text-align: center;
      }

      .cards {
        display: flex;
        justify-content: center;
        gap: 20px;
        margin: 30px 0;
        flex-wrap: wrap;
      }

      .card {
        background: white;
        box-shadow: 0 4px 8px rgb(0 0 0 / 0.1);
        border-radius: 12px;
        padding: 25px 40px;
        width: 220px;
        min-height: 120px;
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        transition: transform 0.2s ease;
      }

      .card:hover {
        transform: translateY(-5px);
        box-shadow: 0 6px 14px rgb(0 0 0 / 0.15);
      }

      .card h3 {
        margin-bottom: 15px;
        font-weight: 700;
        font-size: 1.3rem;
        color: #333;
      }

      .card p {
        font-size: 2.4rem;
        font-weight: 900;
        color: #4285f4;
        margin: 0;
      }

      .restart-container {
        margin-top: 40px;
      }

      .btn-restart {
        padding: 14px 28px;
        background-color: #f44336;
        color: white;
        border: none;
        border-radius: 10px;
        font-weight: 700;
        font-size: 1.1rem;
        cursor: pointer;
        transition: background-color 0.3s ease;
      }

      .btn-restart:hover {
        background-color: #d32f2f;
      }
    `}</style>
  </div>
);

const HomePage: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [players, setPlayers] = useState<Player[]>([]);
  const [activePanel, setActivePanel] = useState<'home' | 'registered' | 'current'>('home');

  const fetchUsers = async () => {
    try {
      const res = await fetch('http://localhost:3000/users');
      const data = await res.json();
      setUsers(data);
    } catch (err) {
      console.error('Błąd pobierania zarejestrowanych użytkowników:', err);
    }
  };

  const fetchPlayers = async () => {
    try {
      const res = await fetch('http://localhost:3000/players');
      const data = await res.json();
      setPlayers(data);
    } catch (err) {
      console.error('Błąd pobierania aktualnych graczy:', err);
    }
  };

  useEffect(() => {
    fetchUsers();
    fetchPlayers();

    const interval = setInterval(() => {
      fetchUsers();
      fetchPlayers();
    }, 5000);

    return () => clearInterval(interval);
  }, []);

  const disconnectPlayer = async (playerId: string) => {
    if (!confirm(`Czy na pewno chcesz rozłączyć gracza ${playerId}?`)) return;

    try {
      const res = await fetch(`http://localhost:3000/player/${playerId}`, {
        method: 'DELETE',
      });

      if (res.ok) {
        alert('Gracz rozłączony.');
        fetchPlayers();
      } else {
        alert('Nie udało się rozłączyć gracza.');
      }
    } catch (err) {
      console.error('Błąd przy rozłączaniu gracza:', err);
      alert('Błąd przy rozłączaniu gracza.');
    }
  };

  const restartServer = async () => {
    if (!confirm('Czy na pewno chcesz zrestartować serwer?')) return;

    try {
      const res = await fetch('http://localhost:3000/restart', { method: 'POST' });
      if (res.ok) {
        alert('Serwer został zrestartowany.');
      } else {
        alert('Nie udało się zrestartować serwera.');
      }
    } catch (err) {
      console.error('Błąd podczas restartu serwera:', err);
      alert('Błąd podczas restartu serwera.');
    }
  };

  return (
    <div className="layout">
      <div className="left-panel">
        <button
          className={activePanel === 'home' ? 'active-btn' : ''}
          onClick={() => setActivePanel('home')}
        >
          Home
        </button>
        <button
          className={activePanel === 'registered' ? 'active-btn' : ''}
          onClick={() => setActivePanel('registered')}
        >
          Registered Users
        </button>
        <button
          className={activePanel === 'current' ? 'active-btn' : ''}
          onClick={() => setActivePanel('current')}
        >
          Current Users
        </button>
      </div>

      <div className="right-panel">
        {activePanel === 'home' && (
          <HomePanel
            usersCount={users.length}
            playersCount={players.length}
            onRestart={restartServer}
          />
        )}
        {activePanel === 'registered' && <RegisteredUsersPanel users={users} />}
        {activePanel === 'current' && (
          <CurrentUsersPanel players={players} onDisconnect={disconnectPlayer} />
        )}
      </div>

      <style jsx>{`
        .layout {
          display: flex;
          height: 100vh;
          background-color: #f4f4f4;
          font-family: Arial, sans-serif;
        }

        .left-panel {
          width: 150px;
          background: #fff;
          border-right: 1px solid #ccc;
          padding: 10px;
          display: flex;
          flex-direction: column;
          gap: 10px;
          box-sizing: border-box;
        }

        .left-panel button {
          padding: 10px;
          border: none;
          background-color: #eee;
          border-radius: 6px;
          cursor: pointer;
          font-weight: 600;
          transition: background-color 0.3s ease;
        }

        .left-panel button:hover {
          background-color: #ddd;
        }

        .active-btn {
          background-color: #4285f4;
          color: white;
        }

        .right-panel {
          flex: 1;
          padding: 20px;
          overflow-y: auto;
          box-sizing: border-box;
        }
      `}</style>
    </div>
  );
};

export default HomePage;
