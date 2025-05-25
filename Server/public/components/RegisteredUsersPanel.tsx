'use client';

import React from 'react';
import { useRouter } from 'next/navigation';

type User = {
  playerId: string;
  username: string;
  password: string;
};

type Props = {
  users: User[];
};

const RegisteredUsersPanel: React.FC<Props> = ({ users }) => {
  const router = useRouter();

  return (
    <>
      <h2>Zarejestrowani gracze: {users.length}</h2>
      <div className="users-grid">
        {users.map((user) => (
          <div key={user.playerId} className="user-card">
            <div className="user-info">
              <p><strong>Player ID:</strong> <span className="ellipsis">{user.playerId}</span></p>
              <p><strong>Username:</strong> <span className="ellipsis">{user.username}</span></p>
              <p><strong>Password:</strong> <span className="ellipsis">{user.password}</span></p>
            </div>
            <div className="user-actions">
              <button
                className="btn-details"
                onClick={() => router.push(`/player/${user.playerId}`)}
              >
                Szczegóły
              </button>
            </div>
          </div>
        ))}
      </div>

      <style jsx>{`
        .users-grid {
          display: grid;
          grid-template-columns: repeat(4, 1fr);
          gap: 16px;
        }

        .user-card {
          background: white;
          border: 1px solid #ccc;
          border-radius: 8px;
          padding: 16px;
          height: 15 0px;
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          box-shadow: 0 2px 6px rgba(0,0,0,0.1);
        }

        .user-info p {
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

        .user-actions {
          display: flex;
          justify-content: flex-end;
          margin-top: 12px;
        }

        .btn-details {
          padding: 8px 16px;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-weight: 600;
          background-color: #3498db;
          color: white;
          transition: background-color 0.2s ease;
        }

        .btn-details:hover {
          background-color: #217dbb;
        }

        /* Responsywność */
        @media (max-width: 900px) {
          .users-grid {
            grid-template-columns: repeat(2, 1fr);
          }
        }
        @media (max-width: 600px) {
          .users-grid {
            grid-template-columns: 1fr;
          }
        }
      `}</style>
    </>
  );
};

export default RegisteredUsersPanel;
