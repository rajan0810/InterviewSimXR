import React from 'react';
import { useLocation, Navigate } from 'react-router-dom';
import './report.css';

const Report = () => {
  const location = useLocation();
  const reportData = location.state?.reportData;

  if (!reportData) {
    return <Navigate to="/report-input" replace />;
  }

  const formatValue = (value) => {
    if (typeof value === 'number') {
      return value.toFixed(2);
    }
    return value;
  };

  return (
    <div className="report-container">
      <div className="report-header-card">
        <h1>Interview Performance Report</h1>
        <p>Report Code: <strong>{reportData.access_code}</strong></p>
        <div className="confidence-score">
          Overall Confidence Score: {(reportData.confidence_score * 100).toFixed(1)}%
        </div>
      </div>

      <div className="report-card">
        <h2>Voice Metrics Analysis</h2>
        <div className="metrics-grid">
          <div className="metric-item">
            <span className="metric-label">Average Confidence</span>
            <span className="metric-value">
              {formatValue(reportData.voice_metrics_summary.avg_confidence)}
            </span>
          </div>
          <div className="metric-item">
            <span className="metric-label">Speech Rate (words/sec)</span>
            <span className="metric-value">
              {formatValue(reportData.voice_metrics_summary.avg_speech_rate)}
            </span>
          </div>
          <div className="metric-item">
            <span className="metric-label">Average Pause (ms)</span>
            <span className="metric-value">
              {formatValue(reportData.voice_metrics_summary.avg_pause)}
            </span>
          </div>
          <div className="metric-item">
            <span className="metric-label">Pitch Consistency</span>
            <span className="metric-value">
              {formatValue(reportData.voice_metrics_summary.pitch_consistency)}
            </span>
          </div>
          <div className="metric-item">
            <span className="metric-label">Energy Consistency</span>
            <span className="metric-value">
              {formatValue(reportData.voice_metrics_summary.energy_consistency)}
            </span>
          </div>
        </div>
      </div>

      <div className="report-card">
        <h2>Interview Summary</h2>
        <div className="summary-grid">
          <div className="summary-item">
            <span className="label">Total Questions</span>
            <span className="value">{reportData.qa_summary.total_questions}</span>
          </div>
          <div className="summary-item">
            <span className="label">Average Answer Length</span>
            <span className="value">
              {formatValue(reportData.qa_summary.avg_answer_length)} words
            </span>
          </div>
          <div className="summary-item">
            <span className="label">Interview Duration</span>
            <span className="value">{reportData.qa_summary.total_interview_duration}</span>
          </div>
        </div>
      </div>

      <div className="report-card">
        <h2>Detailed Analysis</h2>
        <div className="analysis-content">
          {reportData.detailed_analysis.split('\n').map((paragraph, index) => (
            <p key={index}>{paragraph}</p>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Report;