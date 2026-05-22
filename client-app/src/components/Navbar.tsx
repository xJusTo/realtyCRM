import React from 'react';
import { NavLink } from 'react-router-dom';
import { LayoutDashboard, Code, Info, LogOut, User as UserIcon } from 'lucide-react';
import type { User } from '../types/models';

interface Props {
  user: User;
  onLogout: () => void;
}

const Navbar: React.FC<Props> = ({ user, onLogout }) => {
  const getRoleBadge = (role: 'Client' | 'Realtor') => {
    if (role === 'Realtor') {
      return <span className="nav-role-badge realtor">Риэлтор</span>;
    }
    return <span className="nav-role-badge client">Клиент</span>;
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <div className="navbar-brand">🏠 RealtyCRM</div>
        
        <div className="navbar-links">
          <NavLink to="/" className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
            <LayoutDashboard size={18} />
            Панель управления
          </NavLink>
          <NavLink to="/dev" className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
            <Code size={18} />
            Разработчикам
          </NavLink>
          <NavLink to="/about" className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
            <Info size={18} />
            О приложении
          </NavLink>
        </div>

        <div className="navbar-user-profile">
          <div className="user-info-text">
            <span className="user-name">
              <UserIcon size={14} style={{ marginRight: '4px', verticalAlign: 'middle' }} />
              {user.fullName}
            </span>
            {getRoleBadge(user.role)}
          </div>
          <button onClick={onLogout} className="btn-logout" title="Выйти из системы">
            <LogOut size={18} />
            Выйти
          </button>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;

