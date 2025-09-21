using System;

namespace InterviewClient.Models
{
    [Serializable]
    public class StartInterviewRequest
    {
        public string job_role;
        public string job_description; // Optional
    }

    [Serializable]
    public class StartInterviewResponse
    {
        public string session_id;
        public string question_text;
        public string question_audio; // base64 encoded audio
        public int question_number;
    }

    [Serializable]
    public class AnswerQuestionRequest
    {
        public string session_id;
        public string audio_data; // base64 encoded WAV
    }

    [Serializable]
    public class AnswerQuestionResponse
    {
        public string question_text;
        public string question_audio; // base64 encoded audio
        public int? question_number;
        public bool interview_completed;
    }
}