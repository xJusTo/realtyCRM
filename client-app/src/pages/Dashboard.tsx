import React from 'react'
import type { User } from '../types/models'
import ClientDashboard from './ClientDashboard'
import RealtorDashboard from './RealtorDashboard'

interface Props {
  user: User;
}

const Dashboard: React.FC<Props> = ({ user }) => {
  if (user.role === 'Realtor') {
    return <RealtorDashboard />;
  }

  return <ClientDashboard />;
};

export default Dashboard;

