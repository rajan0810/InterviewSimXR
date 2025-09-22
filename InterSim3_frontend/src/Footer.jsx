import React from 'react';
import './Footer.css';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="footer">
      <div className="footer-container">
        <div className="footer-top">
          <div className="footer-left">
            <p>Your partner in professional growth and interview success.</p>
          </div>
          <div className="footer-right">
            <a href="/privacy-policy">Privacy Policy</a>
            <a href="/terms">Terms of Service</a>
            <a href="/support">Support</a>
          </div>
        </div>
        <div className="footer-bottom">
          <p>© {currentYear} InterSimVR. All rights reserved.</p>
        </div>
      </div>
    </footer>
  );
};

export default Footer;