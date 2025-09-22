using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using InterviewClient.Models;
using InterviewClient.Utils;

namespace InterviewClient.Services
{
    public class InterviewClientService : MonoBehaviour
    {
        [SerializeField] private string baseUrl = "https://intersim-xr.onrender.com";

        public event System.Action<StartInterviewResponse> OnInterviewStarted;
        public event System.Action<AnswerQuestionResponse> OnQuestionAnswered;
        public event System.Action<string> OnError;

        /// <summary>
        /// Starts a new interview session using access code as session_id
        /// </summary>
        public void StartInterview(string accessCode, string jobRole)
        {
            StartCoroutine(StartInterviewCoroutine(accessCode, jobRole));
        }

        private IEnumerator StartInterviewCoroutine(string accessCode, string jobRole)
        {
            Debug.Log($"[Interview Service] Starting interview for access code: {accessCode}, role: {jobRole}");

            // ✅ Include access_code and job_role
            var json = new JSONObject();
            json["access_code"] = accessCode;
            json["job_role"] = jobRole;

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json.ToString());

            using (UnityWebRequest webRequest = new UnityWebRequest($"{baseUrl}/interview/start-interview", "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var responseJson = JSON.Parse(webRequest.downloadHandler.text);
                        var response = new StartInterviewResponse
                        {
                            session_id = responseJson["session_id"], // UUID from backend
                            question_text = responseJson["question_text"],
                            question_audio = responseJson["question_audio"],
                            question_number = responseJson["question_number"]
                        };

                        Debug.Log($"[Interview Service] Session started: {response.session_id}");
                        Debug.Log($"[Interview Service] First question: {response.question_text}");

                        OnInterviewStarted?.Invoke(response);
                    }
                    catch (Exception ex)
                    {
                        string error = $"Failed to parse start interview response: {ex.Message}";
                        Debug.LogError($"[Interview Service] {error}");
                        OnError?.Invoke(error);
                    }
                }
                else
                {
                    string error = $"Start interview failed: {webRequest.error}\nResponse: {webRequest.downloadHandler.text}";
                    Debug.LogError($"[Interview Service] {error}");
                    OnError?.Invoke(error);
                }
            }
        }

        /// <summary>
        /// Submits an answer and gets the next question
        /// </summary>
        public void AnswerQuestion(string sessionId, AudioClip audioClip)
        {
            StartCoroutine(AnswerQuestionCoroutine(sessionId, audioClip));
        }

        private IEnumerator AnswerQuestionCoroutine(string sessionId, AudioClip audioClip)
        {
            Debug.Log($"[Interview Service] Submitting answer for session: {sessionId}");

            string audioBase64 = null;
            try
            {
                // Convert audio to base64
                audioBase64 = AudioUtils.ConvertAudioClipToBase64(audioClip);
            }
            catch (Exception ex)
            {
                string error = $"Error processing audio: {ex.Message}";
                Debug.LogError($"[Interview Service] {error}");
                OnError?.Invoke(error);
                yield break; // exit coroutine early
            }

            // ✅ Prepare JSON body with both session_id and access_code
            var json = new JSONObject();
            json["session_id"] = sessionId;
            json["access_code"] = sessionId;   // same as session_id
            json["audio_data"] = audioBase64;

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json.ToString());

            using (UnityWebRequest webRequest = new UnityWebRequest($"{baseUrl}/interview/answer-question", "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[Interview Service] Response: {webRequest.downloadHandler.text}");
                    try
                    {
                        var responseJson = JSON.Parse(webRequest.downloadHandler.text);
                        var response = new AnswerQuestionResponse
                        {
                            interview_completed = responseJson["interview_completed"].AsBool
                        };

                        if (!response.interview_completed)
                        {
                            response.question_text = responseJson["question_text"];
                            response.question_audio = responseJson["question_audio"];
                            response.question_number = responseJson["question_number"].IsNull ? (int?)null : responseJson["question_number"].AsInt;
                        }

                        if (response.interview_completed)
                        {
                            Debug.Log("[Interview Service] Interview completed!");
                        }
                        else
                        {
                            Debug.Log($"[Interview Service] Next question ({response.question_number}): {response.question_text}");
                        }

                        OnQuestionAnswered?.Invoke(response);
                    }
                    catch (Exception ex)
                    {
                        string error = $"Failed to parse answer question response: {ex.Message}";
                        Debug.LogError($"[Interview Service] {error}");
                        OnError?.Invoke(error);
                    }
                }
                else
                {
                    string error = $"Answer question failed: {webRequest.error}\nResponse: {webRequest.downloadHandler.text}";
                    Debug.LogError($"[Interview Service] {error}");
                    OnError?.Invoke(error);
                }
            }
        }
    }
}
