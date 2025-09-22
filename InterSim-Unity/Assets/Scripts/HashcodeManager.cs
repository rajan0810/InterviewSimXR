using UnityEngine;
using TMPro;

public class HashcodeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI codeText;        // drag the TMP text (digit display) here
    public TextMeshProUGUI headerText;      // drag the header TMP text here

    [Header("Door Settings")]
    public Collider entryCollider;          // assign the door knob collider in Inspector
    public string correctCode = "1234";     // set your correct code here

    [Header("Interview Integration")]
    public GameObject canvas;
    public InterviewManager interviewManager; // Reference to interview manager

    void Start()
    {
        if (codeText != null)
            codeText.text = "";

        if (headerText != null)
            headerText.text = "Enter Access Code"; // changed from "Enter Code"

        // Enable the door knob collider at the start
        if (entryCollider != null)
            entryCollider.enabled = true;

        // Find interview manager if not assigned
        if (interviewManager == null)
            interviewManager = FindObjectOfType<InterviewManager>();
    }

    public void AddNum(string num)
    {
        if (codeText != null)
            codeText.text += num;
    }

    public void ClearCode()
    {
        if (codeText != null)
            codeText.text = "";

        if (headerText != null)
            headerText.text = "Enter Access Code";
    }

    public void SubmitCode()
    {
        if (codeText != null && entryCollider != null)
        {
            string enteredCode = codeText.text;

            // Check if code matches (you can modify this logic for dynamic codes)
            if (enteredCode == correctCode || IsValidAccessCode(enteredCode))
            {
                entryCollider.enabled = false;

                if (headerText != null)
                    headerText.text = "Access Granted!";

                Debug.Log($"Access code accepted: {enteredCode}");

                // Start interview with the access code
                if (interviewManager != null)
                {
                    interviewManager.StartInterviewWithCode(enteredCode);
                }
                else
                {
                    Debug.LogError("InterviewManager not found!");
                }

                // Keep canvas active for interview (don't hide it)
                 canvas.SetActive(false); // Commented out to keep interview UI visible
            }
            else
            {
                if (headerText != null)
                    headerText.text = "Invalid Code!";

                Debug.Log("Incorrect access code!");

                // Clear code after a delay
                Invoke(nameof(ClearCodeDelayed), 1.5f);
            }
        }
    }

    private void ClearCodeDelayed()
    {
        ClearCode();
    }

    // Method to validate access codes (you can customize this)
    private bool IsValidAccessCode(string code)
    {
        // For now, accept any 5-digit code
        return code.Length == 5 && System.Text.RegularExpressions.Regex.IsMatch(code, @"^\d{5}$");
    }

    // Optional: Add method to generate random access codes
    public string GenerateAccessCode()
    {
        return Random.Range(10000, 99999).ToString();
    }
}