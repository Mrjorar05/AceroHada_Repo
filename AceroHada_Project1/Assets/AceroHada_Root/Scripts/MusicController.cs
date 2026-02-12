using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    [Header("Scene Music Configuration")]
    [SerializeField] private SceneMusicData[] sceneMusicData;

    private void Start()
    {
        // Reproducir música al cargar la escena
        PlayMusicForCurrentScene();

        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Desuscribirse cuando se destruya el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Buscar configuración por nombre o índice
        foreach (var data in sceneMusicData)
        {
            if (data.sceneName == currentSceneName || data.sceneIndex == currentSceneIndex)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMusic(data.musicIndex);
                }
                return;
            }
        }
    }
}

[System.Serializable]
public class SceneMusicData
{
    public string sceneName;      // Nombre de la escena (ej: "Level3")
    public int sceneIndex = -1;   // Índice en Build Settings (-1 = no usar)
    public int musicIndex;        // Índice de la canción en musicList
}
