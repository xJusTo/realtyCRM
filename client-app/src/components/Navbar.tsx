import React, { useState } from 'react';
import { NavLink } from 'react-router-dom';
import { LayoutDashboard, Code, Info, LogOut, User as UserIcon, Menu, X } from 'lucide-react';
import type { User } from '../types/models';

interface Props {
  user: User;
  onLogout: () => void;
}

const Navbar: React.FC<Props> = ({ user, onLogout }) => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const getRoleBadge = (role: 'Client' | 'Realtor') => {
    if (role === 'Realtor') {
      return <span className="nav-role-badge realtor">Риэлтор</span>;
    }
    return <span className="nav-role-badge client">Клиент</span>;
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <div className="navbar-brand">
          <img src="/logo.svg" alt="RealtyCRM Logo" className="navbar-logo" />
        </div>

        <button 
          className="navbar-toggle" 
          onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
          aria-label="Toggle navigation menu"
        >
          {isMobileMenuOpen ? <X size={24} /> : <Menu size={24} />}
        </button>
        
        <div className={`navbar-menu ${isMobileMenuOpen ? 'is-active' : ''}`}>
          <div className="navbar-links">
            <NavLink to="/" onClick={() => setIsMobileMenuOpen(false)} className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
              <LayoutDashboard size={18} />
              Панель управления
            </NavLink>
            <NavLink to="/dev" onClick={() => setIsMobileMenuOpen(false)} className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
              <Code size={18} />
              Разработчикам
            </NavLink>
            <NavLink to="/about" onClick={() => setIsMobileMenuOpen(false)} className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
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
            <button onClick={() => { onLogout(); setIsMobileMenuOpen(false); }} className="btn-logout" title="Выйти из системы">
              <LogOut size={18} />
              Выйти
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;

