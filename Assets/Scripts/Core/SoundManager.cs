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

    [Header("SFX Clips")]
    public AudioClip appleBasketSound;
    public AudioClip applePickSound;
    public AudioClip chocolateBiteSound;
    public AudioClip digitPressSound;
    public AudioClip digitGoodSound;
    public AudioClip digitWrongSound;
    public AudioClip keycardValidateSound;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("SFX Settings")]
    // Global multiplier applied to all SFX PlayOneShot volumeScale (use inspector to tweak)
    public float sfxGlobalMultiplier = 1.5f;
    // (debug flags removed)

    private bool miniGameMusicPlaying = false;
    private Coroutine fadeCoroutine;
    private float fadeDuration = 0.5f;

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

        if (SceneManager.GetActiveScene().name == SceneFlow.MainMenuScene)
        {
            PlayMenuMusic();
        }
        if (musicSource != null && sfxSource != null && musicSource == sfxSource)
        {
            AudioSource newSfx = gameObject.AddComponent<AudioSource>();
            newSfx.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            newSfx.playOnAwake = false;
            newSfx.loop = false;
            newSfx.volume = sfxSource.volume;
            newSfx.spatialBlend = sfxSource.spatialBlend;
            sfxSource = newSfx;
        }
    }

    public void PlayMenuMusic()
    {
        if (musicSource == null) return;
        if (musicSource.clip != menuMusic)
        {
            StartFadeToClip(menuMusic, true);
            miniGameMusicPlaying = false;
        }
    }

    public void PlayQuestionMusic()
    {
        if (musicSource == null) return;
        if (musicSource.clip != questionMusic)
        {
            StartFadeToClip(questionMusic, true);
            miniGameMusicPlaying = false;
        }
    }

    public void PlayMiniGameMusic()
    {
        if (musicSource == null) return;
        if (!miniGameMusicPlaying)
        {
            StartFadeToClip(miniGameMusic, true);
            miniGameMusicPlaying = true;
        }
    }

    public void PlayGameOverMusic()
    {
        if (musicSource == null) return;
        if (gameOverMusic == null)
        {
            return;
        }
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
        PlaySFX(clip, 1f);
    }

    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (sfxSource == null || clip == null) return;
        float finalScale = Mathf.Clamp(volumeScale * sfxGlobalMultiplier, 0f, 3f);
        sfxSource.PlayOneShot(clip, finalScale);
    }

    public void PlayBasketSound() { PlaySFX(appleBasketSound); }
    public void PlayFruitPickSound() { PlaySFX(applePickSound); }
    public void PlayChocolateBite() { PlaySFX(chocolateBiteSound); }
    public void PlayDigitPress() { PlaySFX(digitPressSound); }
    public void PlayDigitGood() { PlaySFX(digitGoodSound); }
    public void PlayDigitWrong() { PlaySFX(digitWrongSound); }
    public void PlayKeycardValidate() { PlaySFX(keycardValidateSound); }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp01(volume)) * 20f);
        else if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        float safeVol = Mathf.Max(volume, 0.0001f);
        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(safeVol) * 20f);
        else if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp(volume, 0f, 2f);
    }

    private void StartFadeToClip(AudioClip newClip, bool loop)
    {
        if (musicSource == null) return;
        
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