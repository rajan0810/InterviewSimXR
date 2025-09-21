using System;
using System.Collections;
using UnityEngine;

namespace InterviewClient.Utils
{
    public static class AudioUtils
    {
        /// <summary>
        /// Converts AudioClip to base64 encoded WAV format
        /// </summary>
        public static string ConvertAudioClipToBase64(AudioClip audioClip)
        {
            try
            {
                byte[] wavData = WavUtility.FromAudioClip(audioClip);
                return Convert.ToBase64String(wavData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error converting audio to base64: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Converts base64 audio data to AudioClip
        /// </summary>
        public static AudioClip ConvertBase64ToAudioClip(string base64Audio)
        {
            try
            {
                byte[] audioBytes = Convert.FromBase64String(base64Audio);
                return WavUtility.ToAudioClip(audioBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error converting base64 to audio: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Records audio from microphone and returns AudioClip
        /// </summary>
        public static AudioClip RecordAudio(string microphoneName, int lengthSec, int frequency = 44100)
        {
            try
            {
                return Microphone.Start(microphoneName, false, lengthSec, frequency);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error recording audio: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Plays an AudioClip using the provided AudioSource
        /// </summary>
        public static void PlayAudioClip(AudioSource audioSource, AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    // WAV Utility class for converting AudioClip to/from WAV format
    public static class WavUtility
    {
        public static byte[] FromAudioClip(AudioClip audioClip)
        {
            var samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);

            var sampleCount = samples.Length;
            var byteArray = new byte[sampleCount * 2 + 44];

            // WAV header
            WriteStringToByteArray(byteArray, 0, "RIFF");
            WriteInt32ToByteArray(byteArray, 4, byteArray.Length - 8);
            WriteStringToByteArray(byteArray, 8, "WAVE");
            WriteStringToByteArray(byteArray, 12, "fmt ");
            WriteInt32ToByteArray(byteArray, 16, 16);
            WriteInt16ToByteArray(byteArray, 20, 1);
            WriteInt16ToByteArray(byteArray, 22, (short)audioClip.channels);
            WriteInt32ToByteArray(byteArray, 24, audioClip.frequency);
            WriteInt32ToByteArray(byteArray, 28, audioClip.frequency * audioClip.channels * 2);
            WriteInt16ToByteArray(byteArray, 32, (short)(audioClip.channels * 2));
            WriteInt16ToByteArray(byteArray, 34, 16);
            WriteStringToByteArray(byteArray, 36, "data");
            WriteInt32ToByteArray(byteArray, 40, sampleCount * 2);

            // Audio data
            for (int i = 0; i < sampleCount; i++)
            {
                var sample = Mathf.Clamp(samples[i], -1f, 1f);
                var intSample = (short)(sample * short.MaxValue);
                WriteInt16ToByteArray(byteArray, 44 + i * 2, intSample);
            }

            return byteArray;
        }

        public static AudioClip ToAudioClip(byte[] wavData)
        {
            // Parse WAV header
            var channels = BitConverter.ToInt16(wavData, 22);
            var frequency = BitConverter.ToInt32(wavData, 24);
            var dataStart = 44;
            var dataLength = wavData.Length - dataStart;
            var sampleCount = dataLength / 2 / channels;

            var samples = new float[sampleCount * channels];
            for (int i = 0; i < sampleCount * channels; i++)
            {
                var sample = BitConverter.ToInt16(wavData, dataStart + i * 2);
                samples[i] = sample / (float)short.MaxValue;
            }

            var audioClip = AudioClip.Create("ReceivedAudio", sampleCount, channels, frequency, false);
            audioClip.SetData(samples, 0);
            return audioClip;
        }

        private static void WriteStringToByteArray(byte[] array, int offset, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                array[offset + i] = (byte)value[i];
            }
        }

        private static void WriteInt32ToByteArray(byte[] array, int offset, int value)
        {
            array[offset] = (byte)(value & 0xFF);
            array[offset + 1] = (byte)((value >> 8) & 0xFF);
            array[offset + 2] = (byte)((value >> 16) & 0xFF);
            array[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        private static void WriteInt16ToByteArray(byte[] array, int offset, short value)
        {
            array[offset] = (byte)(value & 0xFF);
            array[offset + 1] = (byte)((value >> 8) & 0xFF);
        }
    }
}