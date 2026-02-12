using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    public int musicIndex;

    void Start()
    {
        AudioManager.Instance.PlayMusic(musicIndex);
    }
}
