using UnityEngine;
using OpenAI.Audio;
using System;
using System.Threading.Tasks;
using System.Threading;

public class TTSOpenAIController : MonoBehaviour
{
    private static class WavUtility
    {
        // Convert a byte array to an AudioClip
        public static AudioClip ToAudioClip(byte[] wavFile, string clipName = "wav")
        {
            int sampleRate = BitConverter.ToInt32(wavFile, 24);
            int channels = wavFile[22];

            int pos = 44;
            var samples = new float[(wavFile.Length - pos) / 2];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = BitConverter.ToInt16(wavFile, pos) / 32768f;
                pos += 2;
            }

            var audioClip = AudioClip.Create(clipName, samples.Length, channels, sampleRate, false);
            audioClip.SetData(samples, 0);
            return audioClip;
        }
    }

    private byte[] _currentAudioBytes;

    public async Task TextToSpeechAsync(string input)
    {
        AudioClient client = new("tts-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine));

        BinaryData speech = await client.GenerateSpeechAsync(input, GeneratedSpeechVoice.Echo, new SpeechGenerationOptions()
        {
            SpeedRatio = 1.1f,
            ResponseFormat = GeneratedSpeechFormat.Wav,
        });

        _currentAudioBytes = speech.ToArray();
    }

    public AudioClip GetAudioClip()
    {
        return WavUtility.ToAudioClip(_currentAudioBytes);
    }
}
