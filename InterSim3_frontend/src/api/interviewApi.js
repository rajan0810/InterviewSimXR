import { API_BASE_URL } from "../config";

// Setup Interview API
export async function setupInterview(jobRole, jobDescription, resumeText) {
  const response = await fetch(`${API_BASE_URL}/setup-interview`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      job_role: jobRole,
      job_description: jobDescription,
      resume_text: resumeText,
    }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    console.error('API Error Response:', {
      status: response.status,
      statusText: response.statusText,
      errorData
    });
    throw new Error(`Failed to setup interview: ${response.status} ${response.statusText}`);
  }

  return await response.json(); // { access_code, message }
}
