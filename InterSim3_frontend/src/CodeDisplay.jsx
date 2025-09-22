import React, { useState } from 'react';
import './CodeDisplay.css';

const CodeDisplay = ({ code }) => {
    const [copied, setCopied] = useState(false);

    const handleCopy = () => {
        navigator.clipboard.writeText(code);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    };

    return (
        <div className="code-display-card">
            <div className="card-header">
                <h3>Your Report Code</h3>
                <button onClick={handleCopy} className="copy-button">
                    {copied ? 'Copied!' : 'Copy'}
                </button>
            </div>
            <div className="code-block">
                <pre><code>
                    {code 
                        ? code 
                        : 'Your unique report code will appear here...'}
                </code></pre>
            </div>
        </div>
    );
};

export default CodeDisplay;