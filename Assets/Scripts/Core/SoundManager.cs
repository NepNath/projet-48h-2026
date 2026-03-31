using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Mixer")] 
    public AudioMixer audioMixer;

    private bool miniGameMusicPlaying = false;

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
    }

    public void PlayMenuMusic()
    {
        if (musicSource.clip != menuMusic)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
            miniGameMusicPlaying = false;
        }
    }

    public void PlayQuestionMusic()
    {
        if (musicSource.clip != questionMusic)
        {
            musicSource.clip = questionMusic;
            musicSource.loop = true;
            musicSource.Play();
            miniGameMusicPlaying = false;
        }
    }

    public void PlayMiniGameMusic()
    {
        if (!miniGameMusicPlaying)
        {
            musicSource.clip = miniGameMusic;
            musicSource.loop = true;
            musicSource.Play();
            miniGameMusicPlaying = true;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        miniGameMusicPlaying = false;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
