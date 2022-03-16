using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic BackgroundMusicInstance { get; private set; }
    private AudioSource _audioSource;

    private void Awake()
    {
        if (BackgroundMusicInstance == null) {
            BackgroundMusicInstance = this;
            DontDestroyOnLoad (gameObject);
        }
        else if (BackgroundMusicInstance != this)
        {
            Destroy (gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        _audioSource.mute = PlayerPrefs.GetInt("MuteBG") == 1;
    }
}
