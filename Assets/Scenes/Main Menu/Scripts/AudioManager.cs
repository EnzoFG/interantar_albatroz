using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicaFundoSource;
    public AudioSource efeitosSonorosSource;

    public AudioClip clickSoundClip;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayClickSound()
    {
        if (efeitosSonorosSource != null && efeitosSonorosSource.gameObject != null)
        {
            if (clickSoundClip != null)
            {
                efeitosSonorosSource.PlayOneShot(clickSoundClip);
            }
        }
    }
}