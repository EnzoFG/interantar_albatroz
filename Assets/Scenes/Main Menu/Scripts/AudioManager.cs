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
        // Verifica se a fonte existe E se o objeto do jogo ainda está vivo na memória
        if (efeitosSonorosSource != null && efeitosSonorosSource.gameObject != null)
        {
            // Verifica se o clipe de áudio existe
            if (clickSoundClip != null)
            {
                efeitosSonorosSource.PlayOneShot(clickSoundClip);
            }
        }
    }
}