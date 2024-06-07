using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SFXManagerScript : MonoBehaviour
{
    // Sound effects folder
    public string relativeFolderPath; // Relative path to the folder
    public AudioClip[] sfxArray; // Array to store sound effects
    private string[] sfxFileNames; // Array to store sound effect file names

    void LoadSFX()
    {
        string folderPath = Path.Combine(Application.dataPath, relativeFolderPath); // Convert relative path to absolute path

        string[] files = Directory.GetFiles(folderPath, "*.wav"); // Get all .wav files in the folder

        sfxArray = new AudioClip[files.Length + 1]; // Initialize the array
        sfxFileNames = new string[files.Length + 1]; // Initialize the file names array

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file); // Get file name without extension

            // Extract number from file name
            string[] splitName = fileName.Split('-');
            if (splitName.Length > 1)
            {
                int index;
                if (int.TryParse(splitName[0], out index)) // Convert to number
                {
                    // Load audio clip
                    AudioClip clip = LoadAudioClipFromFile(file);
                    if (clip != null)
                    {
                        // Assign to the array
                        sfxArray[index] = clip;
                        sfxFileNames[index] = fileName; // Store the file name
                    }
                }
            }
        }
    }

    AudioClip LoadAudioClipFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        WAV wav = new WAV(fileData);
        AudioClip audioClip = AudioClip.Create(Path.GetFileNameWithoutExtension(filePath), wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
        audioClip.SetData(wav.GetStereoData(), 0);
        return audioClip;
    }

    // WAV class to convert byte array to AudioClip
    public class WAV
    {
        public int ChannelCount { get; private set; }
        public int SampleCount { get; private set; }
        public int Frequency { get; private set; }
        private float[] LeftChannel;
        private float[] RightChannel;

        public WAV(byte[] wav)
        {
            // Parse the wav file
            ChannelCount = wav[22]; // Channel count
            Frequency = BitConverter.ToInt32(wav, 24); // Sample rate
            int pos = 12; // First Subchunk
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97)) // "data" subchunk
            {
                pos += 4;
                int chunkSize = BitConverter.ToInt32(wav, pos);
                pos += 4 + chunkSize;
            }
            pos += 8;
            SampleCount = (wav.Length - pos) / (2 * ChannelCount);

            LeftChannel = new float[SampleCount];
            if (ChannelCount == 2)
                RightChannel = new float[SampleCount];

            for (int i = 0; i < SampleCount; i++)
            {
                LeftChannel[i] = BitConverter.ToInt16(wav, pos) / 32768.0f;
                pos += 2;
                if (ChannelCount == 2)
                {
                    RightChannel[i] = BitConverter.ToInt16(wav, pos) / 32768.0f;
                    pos += 2;
                }
            }
        }

        public float[] GetStereoData()
        {
            if (ChannelCount == 1)
                return LeftChannel;

            float[] stereoData = new float[SampleCount * 2];
            for (int i = 0; i < SampleCount; i++)
            {
                stereoData[i * 2] = LeftChannel[i];
                stereoData[i * 2 + 1] = RightChannel[i];
            }
            return stereoData;
        }
    }

    // Play sound effect by ID
    public void PlaySFX(int soundID)
    {
        if (soundID < 0 || soundID >= sfxArray.Length || sfxArray[soundID] == null)
        {
            Debug.LogWarning("Invalid sound ID or sound not loaded: " + soundID);
            return;
        }

        // Create a new GameObject for playing the sound
        string objectName = "SFX_" + sfxFileNames[soundID];
        GameObject soundObject = new GameObject(objectName);
        soundObject.transform.parent = this.transform;

        // Add AudioSource component and play the sound
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = sfxArray[soundID];
        audioSource.Play();

        // Start coroutine to fade out and destroy the GameObject after the sound has finished playing
        StartCoroutine(FadeOutAndDestroy(audioSource, 0.9f)); // Adjust fade out duration as needed
    }

    private IEnumerator FadeOutAndDestroy(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float startTime = Time.time;

        while (audioSource.isPlaying)
        {
            float elapsed = Time.time - startTime;
            if (elapsed < audioSource.clip.length - fadeDuration)
            {
                yield return null;
                continue;
            }

            float fadeProgress = (elapsed - (audioSource.clip.length - fadeDuration)) / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, -1f, fadeProgress);
            yield return null;
        }

        // Ensure the audio source is stopped and volume is set to 0
        audioSource.Stop();
        audioSource.volume = 0;

        // Destroy the GameObject
        Destroy(audioSource.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSFX();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
