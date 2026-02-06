using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Declaracion Singleton
    private static AudioManager instance; //Definicion de la fortaleza de datos
    public static AudioManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("No hay Game Manager");
            return instance;
        }
    }
    //Fin del singleton

    //TODA LAS VARIABLES D ELA FORTALEZA DEBAN SER PUBLICAS
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip[] musicLibrary;
    public AudioClip[] sfxLibrary;

    private void Awake()
    {
        if (instance != null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayMusic(int musicToPlay)
    {
        musicSource.clip = musicLibrary[musicToPlay];
        musicSource.Play();
    }
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    } 

    public void PlaySFX(int sfxToPlay)
    {
        sfxSource.PlayOneShot(sfxLibrary[sfxToPlay]);
    }
    //Prueba
}
