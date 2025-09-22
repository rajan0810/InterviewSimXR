import React, { useState } from 'react';
import './UploadForm.css';
import { setupInterview } from './api/interviewApi';

const UploadForm = ({ onCodeGenerated }) => {
    const [file, setFile] = useState(null);
    const [companyName, setCompanyName] = useState('');
    const [jobRole, setJobRole] = useState('');
    const [isDragging, setIsDragging] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    const handleFileChange = (e) => {
        const selectedFile = e.target.files[0];
        if (selectedFile && selectedFile.type !== 'application/pdf') {
            alert('Please select a PDF file only.');
            e.target.value = ''; // Clear the file input
            return;
        }
        setFile(selectedFile);
        console.log('File selected:', {
            name: selectedFile.name,
            size: selectedFile.size,
            type: selectedFile.type
        });
    };

    const handleDrop = (e) => {
        e.preventDefault();
        setIsDragging(false);
        if (e.dataTransfer.files && e.dataTransfer.files[0]) {
            setFile(e.dataTransfer.files[0]);
        }
    };

    const handleDragOver = (e) => {
        e.preventDefault();
        setIsDragging(true);
    };

    const handleDragLeave = () => {
        setIsDragging(false);
    };



    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!file || !companyName || !jobRole) {
            alert('Please fill out all fields and select a file.');
            return;
        }

        if (file.type !== 'application/pdf') {
            alert('Please upload a PDF file only.');
            return;
        }

        setIsLoading(true);
        
        try {
            // Read file as text using FileReader
            const reader = new FileReader();
            reader.onload = async (event) => {
                try {
                    const resumeText = event.target.result;
                    
                    // Call the setup interview API
                    const response = await setupInterview(
                        jobRole,
                        `Position at ${companyName}: ${jobRole}`,
                        resumeText
                    );
                    
                    if (response && response.access_code) {
                        onCodeGenerated(response.access_code);
                        console.log('Setup successful:', response.message);
                    }
                } catch (error) {
                    console.error('API call failed:', error);
                    alert('Failed to setup interview. Please try again.');
                } finally {
                    setIsLoading(false);
                }
            };
            
            reader.onerror = () => {
                console.error('File reading failed');
                alert('Failed to read the file. Please try again.');
                setIsLoading(false);
            };
            
            reader.readAsText(file);
        } catch (error) {
            console.error('Upload failed:', error);
            alert('File upload failed. Please try again.');
            setIsLoading(false);
        }
    };

    return (
        <div className="upload-card">
            <h2>Start a New Interview Simulation</h2>
            <form onSubmit={handleSubmit}>
                <div className="input-group">
                    <label htmlFor="company-name">Company Name</label>
                    <input
                        type="text"
                        id="company-name"
                        value={companyName}
                        onChange={(e) => setCompanyName(e.target.value)}
                        placeholder="e.g., Google"
                    />
                </div>
                <div className="input-group">
                    <label htmlFor="job-role">Job Role</label>
                    <input
                        type="text"
                        id="job-role"
                        value={jobRole}
                        onChange={(e) => setJobRole(e.target.value)}
                        placeholder="e.g., Senior Software Engineer"
                    />
                </div>
                <div 
                    className={`drop-area ${isDragging ? 'dragging' : ''}`}
                    onDragOver={handleDragOver}
                    onDragLeave={handleDragLeave}
                    onDrop={handleDrop}
                >
                    <input 
                        type="file" 
                        id="file-upload" 
                        className="file-input" 
                        onChange={handleFileChange} 
                        accept=".pdf" 
                    />
                    <label htmlFor="file-upload">
                        {file ? file.name : 'Drag & Drop Resume Here'}
                    </label>
                </div>
                <button type="submit" disabled={isLoading || !file || !companyName || !jobRole}>
                    {isLoading ? 'Processing...' : 'Submit'}
                </button>
            </form>
        </div>
    );
};

export default UploadForm;