using UnityEngine;
using UnityEngine.SceneManagement;
//manages all of the games audio!

//basic set up...
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip titleMusic;
    public AudioClip cutsceneMusic;
    public AudioClip gameplayMusic;

    [Header("SFX")]
    public AudioClip buttonClick;
    public AudioClip bossHit;
    public AudioClip bossDefeat;
    public AudioClip textboxBlip;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Ensure the AudioManager persists across scene loads
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //finds the scene to get the music for it
    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    //plays the correct music for the scene when the scene is loaded
    private void PlayMusicForScene(string sceneName)
    {
        AudioClip nextClip = null;

        if (sceneName == "TitleScreen")
            nextClip = titleMusic;
        else if (sceneName == "CutScene1" || sceneName == "cutscene1")
            nextClip = cutsceneMusic;
        else if (sceneName == "GameplayScene")
            nextClip = gameplayMusic;

        PlayMusic(nextClip);
    }
    //plays the music if it is not already playing
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayBossHit()
    {
        PlaySFX(bossHit);
    }
     
    public void PlayBossDefeat()
    {
        PlaySFX(bossDefeat);
    }

    //plays the sound effect if it is not null
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}
