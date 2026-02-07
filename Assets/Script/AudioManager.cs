using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // --- SINGLETON PATTERN ---
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;      // Nama unik untuk dipanggil (misal: "Jump", "Attack")
        public AudioClip clip;   // File audio
        [Range(0f, 1f)]
        public float volume = 0.5f;
    }

    [Header("Audio Data")]
    public List<Sound> sfxList = new List<Sound>();
    public List<Sound> bgmList = new List<Sound>();

    private AudioSource bgmSource;
    private AudioSource[] sfxSources = new AudioSource[3];
    private int currentSfxSourceIndex = 0;

    void Awake()
    {
        // Setup Singleton dan Persistence
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // Setup BGM Source
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        // Setup 3 SFX Sources
        for (int i = 0; i < sfxSources.Length; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
        }
    }

    // --- FUNGSI PEMANGGILAN BGM ---

    public void PlayBGM(string soundName)
    {
        Sound s = bgmList.Find(x => x.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("BGM tidak ditemukan: " + soundName);
            return;
        }

        if (bgmSource.clip == s.clip) return; // Jangan restart jika lagu yang sama

        bgmSource.clip = s.clip;
        bgmSource.volume = s.volume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- FUNGSI PEMANGGILAN SFX ---

    public void PlaySFX(string soundName)
    {
        Sound s = sfxList.Find(x => x.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("SFX tidak ditemukan: " + soundName);
            return;
        }

        // Mencari source yang sedang tidak memutar lagu, 
        // atau gunakan sistem rotasi jika semua penuh.
        AudioSource currentSource = sfxSources[currentSfxSourceIndex];
        
        currentSource.clip = s.clip;
        currentSource.volume = s.volume;
        currentSource.Play();

        // Rotasi index (0, 1, 2, kembali ke 0)
        currentSfxSourceIndex = (currentSfxSourceIndex + 1) % sfxSources.Length;
    }
}