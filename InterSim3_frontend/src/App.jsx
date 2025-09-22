import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Header from './Header';
import MainContent from './MainContent';
import UploadForm from './UploadForm';
import CodeDisplay from './CodeDisplay';
import ReportInput from './ReportInput';
import Report from './report';
import Footer from './Footer';
import About from './About';
import InterviewSetup from "./pages/InterviewSetup";
import './App.css';

function App() {
  const [generatedCode, setGeneratedCode] = useState('');

  const handleCodeGeneration = (code) => {
    setGeneratedCode(code);
  };

  return (
    <Router>
      <div className="app-container">
        <Header />
        <main className="app-main">
          <Routes>
            <Route path="/" element={
              <div className="main-layout">
                <MainContent />
                <div className="upload-section">
                  {!generatedCode ? (
                    <UploadForm onCodeGenerated={handleCodeGeneration} />
                  ) : (
                    <CodeDisplay code={generatedCode} />
                  )}
                </div>
              </div>
            } />
            <Route path="/about" element={<About />} />
            <Route path="/report" element={<Report />} />
            <Route path="/report-input" element={<ReportInput />} />
            <Route path="/interview-setup" element={<InterviewSetup />} /> {/* NEW */}
          </Routes>
        </main>
        <Footer />
      </div>
    </Router>
  );
}

export default App;
