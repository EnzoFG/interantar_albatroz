using UnityEngine;

public class MapaAudioController : MonoBehaviour
{
    [Header("Configuração")]
    public AudioClip musicaDoMapa;

    void Start()
    {
        if (AudioManager.instance != null && AudioManager.instance.musicaFundoSource != null)
        {
            if (AudioManager.instance.musicaFundoSource.clip != musicaDoMapa || !AudioManager.instance.musicaFundoSource.isPlaying)
            {
                AudioManager.instance.musicaFundoSource.clip = musicaDoMapa;
                AudioManager.instance.musicaFundoSource.loop = true;
                AudioManager.instance.musicaFundoSource.Play();
            }
        }
    }

    void OnDisable()
    {
        if (AudioManager.instance != null && AudioManager.instance.musicaFundoSource != null)
        {
            AudioManager.instance.musicaFundoSource.Stop();
        }
    }
}