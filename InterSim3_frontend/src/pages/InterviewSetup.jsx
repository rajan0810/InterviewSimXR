import { useState } from "react";
import { setupInterview } from "../api/interviewApi";

function InterviewSetup() {
  const [accessCode, setAccessCode] = useState(null);

  async function handleSetup() {
    try {
      const data = await setupInterview(
        "Software Engineer",
        "Responsible for building scalable web apps",
        "This is the candidate resume text"
      );
      setAccessCode(data.access_code);
    } catch (error) {
      console.error(error);
      alert("Failed to setup interview!");
    }
  }

  return (
    <div className="p-6">
      <button
        onClick={handleSetup}
        className="px-4 py-2 bg-blue-600 text-white rounded-lg"
      >
        Setup Interview
      </button>

      {accessCode && (
        <p className="mt-4">✅ Your Access Code: {accessCode}</p>
      )}
    </div>
  );
}

export default InterviewSetup;
