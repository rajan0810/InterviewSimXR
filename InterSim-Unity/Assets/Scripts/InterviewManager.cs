using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InterviewClient.Services;
using InterviewClient.Models;
using InterviewClient.Utils;

public class InterviewManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI codeText;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI questionText;
    public Button stopRecordingButton;
    public AudioSource audioSource;

    [Header("Interview Settings")]
    public int recordingLength = 30;
    public string jobRole = "Data Scientist";

    private InterviewClientService interviewService;
    private string accessCode;   // 🔑 Use this everywhere instead of sessionId
    private AudioClip currentQuestionAudio;
    private AudioClip recordedAnswer;
    private bool isRecording = false;
    private bool interviewStarted = false;
    private bool interviewCompleted = false;
    private int currentQuestionNumber = 1;

    void Start()
    {
        InitializeInterview();
    }

    private void InitializeInterview()
    {
        interviewService = gameObject.AddComponent<InterviewClientService>();
        interviewService.OnInterviewStarted += OnInterviewStarted;
        interviewService.OnQuestionAnswered += OnQuestionAnswered;
        interviewService.OnError += OnError;

        stopRecordingButton.gameObject.SetActive(false);
        stopRecordingButton.onClick.AddListener(StopRecording);

        headerText.text = "Enter Access Code";
        questionText.text = "Welcome! Please enter your access code above to begin the interview.";
    }

    public void StartInterviewWithCode(string code)
    {
        if (!interviewStarted && !string.IsNullOrEmpty(code))
        {
            accessCode = code; // 🔑 Save the access code
            headerText.text = "Starting Interview...";
            questionText.text = "Please wait while we prepare your interview questions...";
            interviewService.StartInterview(accessCode, jobRole);
        }
    }

    // ------------------ Recording ------------------
    private void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found!");
            headerText.text = "No Microphone Found";
            headerText.color = Color.red;
            return;
        }

        string micName = Microphone.devices[0];
        Debug.Log($"🎙 Using microphone: {micName}");

        recordedAnswer = Microphone.Start(micName, false, recordingLength, 44100);

        if (recordedAnswer == null)
        {
            Debug.LogError("Failed to start recording (AudioClip is null).");
            return;
        }

        isRecording = true;
        stopRecordingButton.gameObject.SetActive(true);

        headerText.text = $"🎙 Recording Answer ({recordingLength}s max)";
        headerText.color = Color.red;

        StartCoroutine(AutoStopRecording());
    }

    public void StopRecording()
    {
        if (!isRecording) return;

        if (Microphone.devices.Length > 0)
            Microphone.End(Microphone.devices[0]);

        isRecording = false;
        stopRecordingButton.gameObject.SetActive(false);

        // 🔎 Verify recorded clip
        if (recordedAnswer == null || recordedAnswer.samples == 0)
        {
            Debug.LogError("No audio recorded. Skipping answer submission.");
            headerText.text = "Recording Failed!";
            headerText.color = Color.red;
            return;
        }

        Debug.Log($"Recorded {recordedAnswer.samples} samples, length: {recordedAnswer.length:F2}s");

        headerText.text = "Processing Answer...";
        headerText.color = Color.yellow;

        if (!string.IsNullOrEmpty(accessCode))
            interviewService.AnswerQuestion(accessCode, recordedAnswer); // ✅ Use accessCode
    }

    private IEnumerator AutoStopRecording()
    {
        float timeLeft = recordingLength;
        while (timeLeft > 0 && isRecording)
        {
            headerText.text = $"Recording... {timeLeft:F0}s left";
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        if (isRecording)
            StopRecording();
    }

    // ------------------ Question Handling ------------------
    private void PlayQuestionAudio()
    {
        if (currentQuestionAudio != null && audioSource != null)
        {
            audioSource.clip = currentQuestionAudio;
            audioSource.Play();
            StartCoroutine(StartRecordingAfterAudio());
        }
        else
        {
            StartRecording(); // No audio, start recording immediately
        }
    }

    private IEnumerator StartRecordingAfterAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length + 0.5f);
        StartRecording();
    }

    // ------------------ Event Handlers ------------------
    private void OnInterviewStarted(StartInterviewResponse response)
    {
        interviewStarted = true;
        currentQuestionNumber = response.question_number;

        headerText.text = $"Question {response.question_number}";
        questionText.text = response.question_text;

        currentQuestionAudio = AudioUtils.ConvertBase64ToAudioClip(response.question_audio);
        PlayQuestionAudio();
    }

    private void OnQuestionAnswered(AnswerQuestionResponse response)
    {
        if (response.interview_completed)
        {
            interviewCompleted = true;
            headerText.text = "Interview Complete!";
            headerText.color = Color.green;
            questionText.text = "Congratulations! You have completed the interview.";
            stopRecordingButton.gameObject.SetActive(false);
        }
        else
        {
            currentQuestionNumber = response.question_number ?? (currentQuestionNumber + 1);
            headerText.text = $"Question {currentQuestionNumber}";
            questionText.text = response.question_text;

            currentQuestionAudio = AudioUtils.ConvertBase64ToAudioClip(response.question_audio);
            PlayQuestionAudio();
        }
    }

    private void OnError(string error)
    {
        headerText.text = "Error Occurred";
        headerText.color = Color.red;
        questionText.text = $"An error occurred: {error}";
        stopRecordingButton.gameObject.SetActive(false);
        isRecording = false;
    }

    private void OnDestroy()
    {
        if (interviewService != null)
        {
            interviewService.OnInterviewStarted -= OnInterviewStarted;
            interviewService.OnQuestionAnswered -= OnQuestionAnswered;
            interviewService.OnError -= OnError;
        }

        if (isRecording && Microphone.devices.Length > 0)
            Microphone.End(Microphone.devices[0]);
    }
}
