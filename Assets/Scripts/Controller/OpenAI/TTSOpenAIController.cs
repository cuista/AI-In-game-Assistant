using UnityEngine;
using OpenAI.Audio;
using System;
using System.Threading.Tasks;

public class TTSOpenAIController : MonoBehaviour
{
    private byte[] currentTTSGeneration;

    public async Task TextToSpeechAsync(string input)
    {
        AudioClient client = new("tts-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine));

        BinaryData speech = await client.GenerateSpeechAsync(input, GeneratedSpeechVoice.Echo, new SpeechGenerationOptions()
        {
            //SpeedRatio = 1.1f,
        });

        currentTTSGeneration = speech.ToArray();
    }

    public byte[] GetCurrentTTSGeneration()
    {
        return currentTTSGeneration;
    }
}
