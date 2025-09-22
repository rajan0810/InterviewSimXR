import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './reportInput.css';

const LoadingSpinner = () => (
  <div className="loading-spinner"></div>
);

const ReportInput = () => {
  const [code, setCode] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (code.length !== 5) {
      setError('Please enter a 5-digit code');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const response = await fetch('https://intersim-xr.onrender.com/interview/generate-report', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          access_code: code
        })
      });

      if (!response.ok) {
        if (response.status === 404) {
          throw new Error('Invalid access code');
        } else if (response.status === 400) {
          const errorData = await response.json();
          throw new Error(errorData.detail || 'Interview not yet completed or no data found');
        } else {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
      }

      const data = await response.json();
      
      // Navigate with the complete report data structure
      navigate('/report', { 
        state: { 
          reportData: {
            access_code: data.access_code,
            confidence_score: data.confidence_score,
            detailed_analysis: data.detailed_analysis,
            voice_metrics_summary: data.voice_metrics_summary,
            qa_summary: data.qa_summary
          }
        } 
      });
    } catch (error) {
      console.error('Error fetching report:', error);
      setError(error.message || 'Error fetching report. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="report-input-container">
      <div className="report-input-box">
        {isLoading && (
          <div className="loading-overlay">
            <LoadingSpinner />
          </div>
        )}
        <h2>View Your Report</h2>
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              type="text"
              value={code}
              onChange={(e) => setCode(e.target.value.slice(0, 5))}
              placeholder="Enter your 5-digit report code"
              maxLength="5"
              pattern="[0-9]{5}"
              disabled={isLoading}
            />
          </div>
          {error && <p className="error-message">{error}</p>}
          <button type="submit" disabled={isLoading}>
            <span className="button-content">
              View Report
              {isLoading && <LoadingSpinner />}
            </span>
          </button>
        </form>
      </div>
    </div>
  );
};

export default ReportInput;