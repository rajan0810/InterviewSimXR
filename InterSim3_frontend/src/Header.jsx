import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css';

const Header = () => {
  return (
    <header className="header">
      <div className="header-container">
        <div className="header-inner">
          <div className="logo">
            <Link to="/">
              <h1>InterSimVR</h1>
            </Link>
          </div>
          <nav className="nav-desktop">
            <div className="nav-links">
              <Link to="/">Home</Link>
              <Link to="/about">About</Link>
              <Link to="/report-input">Report</Link>
            </div>
          </nav>
        </div>
      </div>
    </header>
  );
};

export default Header;