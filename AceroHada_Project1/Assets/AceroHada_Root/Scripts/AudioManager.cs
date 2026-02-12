using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clip Arrays")]
    public AudioClip[] musicList;
    public AudioClip[] sfxList;

    [Header("Scene Music Configuration")]
    [SerializeField] private SceneMusicConfig[] sceneMusicConfig;

    private int currentMusicIndex = -1;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Suscribirse al evento de carga de escenas
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Reproducir música de la escena inicial
        PlayMusicForCurrentScene();
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento al destruir
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cambiar música automáticamente al cargar nueva escena
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Buscar la configuración de música para la escena actual
        foreach (var config in sceneMusicConfig)
        {
            bool matchByName = !string.IsNullOrEmpty(config.sceneName) && config.sceneName == currentSceneName;
            bool matchByIndex = config.sceneIndex >= 0 && config.sceneIndex == currentSceneIndex;

            if (matchByName || matchByIndex)
            {
                PlayMusic(config.musicIndex);
                return;
            }
        }

        // Si no hay configuración específica, no cambiar la música
        Debug.Log($"No se encontró configuración de música para la escena: {currentSceneName}");
    }

    public void PlayMusic(int musicIndex)
    {
        // Validar índice
        if (musicIndex < 0 || musicIndex >= musicList.Length)
        {
            Debug.LogWarning($"Índice de música inválido: {musicIndex}");
            return;
        }

        // Evitar reproducir la misma música si ya está sonando
        if (currentMusicIndex == musicIndex && musicSource.isPlaying)
        {
            return;
        }

        currentMusicIndex = musicIndex;
        musicSource.clip = musicList[musicIndex];
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        currentMusicIndex = -1;
    }

    public void PlaySFX(int sfxIndex)
    {
        // Validar índice
        if (sfxIndex < 0 || sfxIndex >= sfxList.Length)
        {
            Debug.LogWarning($"Índice de SFX inválido: {sfxIndex}");
            return;
        }

        sfxSource.PlayOneShot(sfxList[sfxIndex]);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}

[System.Serializable]
public class SceneMusicConfig
{
    [Tooltip("Nombre exacto de la escena (opcional si usas sceneIndex)")]
    public string sceneName;

    [Tooltip("Índice de la escena en Build Settings (-1 para ignorar)")]
    public int sceneIndex = -1;

    [Tooltip("Índice de la música en el array musicList")]
    public int musicIndex;
}
