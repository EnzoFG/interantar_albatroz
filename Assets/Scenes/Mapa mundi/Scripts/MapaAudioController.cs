using UnityEngine;

public class MapaAudioController : MonoBehaviour
{
    [Header("Configuração")]
    public AudioClip musicaDoMapa;

    void Start()
    {
        // Ao carregar a cena do Mapa, acessamos o AudioManager global e mandamos tocar
        if (AudioManager.instance != null && AudioManager.instance.musicaFundoSource != null)
        {
            // Só troca a música e dá play se ela não estiver tocando (evita reiniciar à toa)
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
        // IMPORTANTE:
        // Quando sairmos do Mapa para ir a um Minigame, paramos a música global.
        // Assim ela não toca junto com a música local do minigame.
        if (AudioManager.instance != null && AudioManager.instance.musicaFundoSource != null)
        {
            AudioManager.instance.musicaFundoSource.Stop();
        }
    }
}