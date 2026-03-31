using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip menuMusic;
    public AudioClip questionMusic;
    public AudioClip miniGameMusic;
    public AudioClip gameOverMusic;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    private bool miniGameMusicPlaying = false;
    private Coroutine fadeCoroutine;
    private float fadeDuration = 0.5f; // seconds

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // If we start on the main menu, ensure menu music plays immediately
        if (SceneManager.GetActiveScene().name == SceneFlow.MainMenuScene)
        {
            PlayMenuMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (musicSource == null) return;
        if (musicSource.clip != menuMusic)
        {
            Debug.Log("[SoundManager] PlayMenuMusic requested");
            StartFadeToClip(menuMusic, true);
            miniGameMusicPlaying = false;
        }
    }

    public void PlayQuestionMusic()
    {
        if (musicSource == null) return;
        if (musicSource.clip != questionMusic)
        {
            Debug.Log("[SoundManager] PlayQuestionMusic requested");
            StartFadeToClip(questionMusic, true);
            miniGameMusicPlaying = false;
        }
    }

    public void PlayMiniGameMusic()
    {
        if (musicSource == null) return;
        if (!miniGameMusicPlaying)
        {
            Debug.Log("[SoundManager] PlayMiniGameMusic requested");
            StartFadeToClip(miniGameMusic, true);
            miniGameMusicPlaying = true;
        }
    }

    public void PlayGameOverMusic()
    {
        if (musicSource == null) return;
        if (gameOverMusic == null)
        {
            Debug.LogWarning("[SoundManager] PlayGameOverMusic requested but no clip assigned.");
            return;
        }
        Debug.Log("[SoundManager] PlayGameOverMusic requested");
        StartFadeToClip(gameOverMusic, true);
        miniGameMusicPlaying = false;
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        miniGameMusicPlaying = false;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
        else if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
        else if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }

    private void StartFadeToClip(AudioClip newClip, bool loop)
    {
        if (musicSource == null) return;
        Debug.Log($"[SoundManager] StartFadeToClip -> { (newClip!=null?newClip.name:"null") }");
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToClipCoroutine(newClip, loop));
    }

    private System.Collections.IEnumerator FadeToClipCoroutine(AudioClip newClip, bool loop)
    {
        float startVolume = musicSource.volume;
        float t = 0f;
        // Fade out
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = 0f;
        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.Play();
        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = startVolume;
        fadeCoroutine = null;
    }
}