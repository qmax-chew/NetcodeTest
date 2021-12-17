using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class VoiceChat : MonoBehaviour
{
    bool pushToTalk = false;
    AudioSource source;
    AudioClip clip;
    int writeHead;

    int space;

    // Start is called before the first frame update
    void Awake()
    {
        var samplingRate = SteamUser.OptimalSampleRate;
        source = gameObject.AddComponent<AudioSource>();
        clip = AudioClip.Create("Voice", (int)samplingRate * 10, 1, (int)samplingRate, false);
        space = (int)samplingRate / 10;
        source.clip = clip;
        source.loop = true;
    }

    public void AddData(byte[] voiceData, uint length)
    {
        if (length == 0) return;

        if (!source.isPlaying)
        {
            source.timeSamples = 0;
            writeHead = 0;
        }

        var convertedData = Convert(voiceData, length);
        clip.SetData(convertedData, writeHead);
        writeHead += convertedData.Length - space;

        if (!source.isPlaying)
        {
            source.volume = 1;
            source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasReadableData())
        {
            source.volume = 0;
            source.Stop();
        }
    }

    float[] Convert(byte[] byteVoice, uint length)
    {
        const int bitDepth = 16;
        var bytesPerSample = bitDepth / 8;
        var floatVoice = new float[length / bytesPerSample + space]; // space ÇæÇØå„ÇÎÇ…ë´ÇµÇƒÉuÉcÉbÇ∆êÿÇÍÇ»Ç¢ÇÊÇ§Ç…
        for (var i = 0; i < floatVoice.Length - space; i++)
        {
            var data = (System.Int16)(byteVoice[i * bytesPerSample] + (byteVoice[i * bytesPerSample + 1] << 8)); // 16bit Little Endian
            floatVoice[i] = data / (System.Int16.MaxValue + 1f); // Max ÇÊÇË Min ÇÃÇŸÇ§Ç™1ëÂÇ´Ç¢ÇÃÇ≈ê≥ãKâªÇ∑ÇÈÇΩÇﬂÇ…ÇÕ + 1 ÇµÇ»Ç¢Ç∆Ç¢ÇØÇ»Ç¢
        }
        return floatVoice;
    }

    bool HasReadableData()
    {
        var half = clip.samples / 2;
        if (source.timeSamples > writeHead)
        {
            return source.timeSamples - writeHead > half;
        }
        else
        {
            return writeHead - source.timeSamples < half;
        }
    }
}
