'use client';

import React from 'react';

type User = {
  playerId: string;
  username: string;
  password: string;
};

type Props = {
  user: User;
};

const UserCard: React.FC<Props> = ({ user }) => {
  return (
    <div className="user-card">
      <p><strong>Player ID:</strong> {user.playerId}</p>
      <p><strong>Username:</strong> {user.username}</p>
      <p><strong>Password:</strong> {user.password}</p>
    </div>
  );
};

export default UserCard;
