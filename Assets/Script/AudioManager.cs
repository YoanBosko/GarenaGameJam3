using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
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

    [System.Serializable]
    public class SceneBGMConfig
    {
        public string sceneName; // Nama Scene di Build Settings
        public string soundName; // Nama BGM yang ada di bgmList
    }

    [Header("Audio Data")]
    public List<Sound> sfxList = new List<Sound>();
    public List<Sound> bgmList = new List<Sound>();

    [Header("Scene BGM Configurations")]
    public List<SceneBGMConfig> sceneBGMConfigs = new List<SceneBGMConfig>();

    private AudioSource bgmSource;
    private AudioSource[] sfxSources = new AudioSource[3];
    private AudioSource loopSfxSource; // Source khusus untuk SFX looping (seperti lari)
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

    private void OnEnable()
    {
        // Berlangganan ke event perpindahan scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Berhenti berlangganan saat object dihancurkan
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

        // Setup 1 Looping SFX Source (Khusus suara berulang)
        loopSfxSource = gameObject.AddComponent<AudioSource>();
        loopSfxSource.loop = true;
        loopSfxSource.playOnAwake = false;
    }

    // Fungsi otomatis yang dipanggil setiap kali scene baru dimuat
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cari apakah ada konfigurasi BGM untuk scene yang baru dimuat
        SceneBGMConfig config = sceneBGMConfigs.Find(x => x.sceneName == scene.name);
        
        if (config != null)
        {
            PlayBGM(config.soundName);
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

        // Jika lagu yang ingin diputar sudah sedang berjalan, abaikan agar tidak restart dari nol
        if (bgmSource.clip == s.clip && bgmSource.isPlaying) return;

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

        // Tambahan Pertama: Cek apakah suara yang sama sedang dimainkan di salah satu source pool
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying && source.clip == s.clip)
            {
                // Jika suara yang sama sedang diputar, abaikan panggilan ini (Anti-Spam)
                return;
            }
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

    // Tambahan Kedua: Fungsi untuk memainkan SFX yang terus berulang (seperti lari)
    public void PlaySFXLoop(string soundName)
    {
        Sound s = sfxList.Find(x => x.name == soundName);
        if (s == null) return;

        // Jika suara loop yang diminta sudah sedang berputar, jangan di-reset
        if (loopSfxSource.clip == s.clip && loopSfxSource.isPlaying) return;

        loopSfxSource.clip = s.clip;
        loopSfxSource.volume = s.volume;
        loopSfxSource.Play();
    }

    // Fungsi untuk mematikan SFX looping
    public void StopSFXLoop()
    {
        loopSfxSource.Stop();
        loopSfxSource.clip = null;
    }
}