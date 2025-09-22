import React from 'react';
import './MainContent.css';

const MainContent = () => {
    return (
        <div className="main-content">
            <div className="content-section">
                <div className="content-header">
                    <h1>Master Your Next Interview with InterSimVR</h1>
                    <p className="content-subtitle">
                        Practice, get AI-powered feedback, and build the confidence you need to succeed.
                    </p>
                </div>
                <div className="features-grid">
                    <div className="feature-card">
                        <h3>Realistic VR Scenarios</h3>
                        <p>Immerse yourself in virtual office environments to practice under realistic pressure.</p>
                    </div>
                    <div className="feature-card">
                        <h3>AI Driven Interviewer</h3>
                        <p>Receive objective analysis on your body language, tone, and filler words.</p>
                    </div>
                    <div className="feature-card">
                        <h3>Customizable Interviews</h3>
                        <p>Tailor your practice sessions to specific job roles and company cultures.</p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default MainContent;