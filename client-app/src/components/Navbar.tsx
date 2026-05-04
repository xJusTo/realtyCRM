import React from 'react';
import { NavLink } from 'react-router-dom';
import { LayoutDashboard, Code, Info } from 'lucide-react';

const Navbar = () => {
  return (
    <nav className="navbar">
      <div className="navbar-container">
        <div className="navbar-brand">RealtyCRM</div>
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
      </div>
    </nav>
  );
};

export default Navbar;
