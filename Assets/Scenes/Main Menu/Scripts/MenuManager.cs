using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Painéis do Menu")]
    public GameObject painelConfirmarSair;
    public GameObject painelCreditos;
    public GameObject painelOpcoes;

    [Header("Áudio")]
    public AudioMixer gameMixer;
    // REMOVIDO: public AudioSource somDeCliqueSource; -> Não precisamos mais disso aqui

    [Header("UI Elementos")]
    public Slider volumeSlider;

    void Start()
    {
        painelConfirmarSair.SetActive(false);
        painelCreditos.SetActive(false);
        painelOpcoes.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (volumeSlider != null) 
        {
            volumeSlider.value = savedVolume;
        }
        SetVolume(savedVolume);
    }

    public void BotaoJogar_Click()
    {
        TocarSomDeClique(); 
        
        // Paramos a música do menu antes de trocar de cena
        if (AudioManager.instance != null && AudioManager.instance.musicaFundoSource != null)
        {
            AudioManager.instance.musicaFundoSource.Stop();
        }

        SceneManager.LoadScene("Mapa Mundi");
    }

    public void BotaoOpcoes_Click()
    {
        TocarSomDeClique();
        painelOpcoes.SetActive(true);
    }

    public void BotaoCreditos_Click()
    {
        TocarSomDeClique();
        painelCreditos.SetActive(true);
    }

    public void BotaoSair_Click()
    {
        TocarSomDeClique();
        painelConfirmarSair.SetActive(true);
    }

    public void BotaoSairSim_Click()
    {
        TocarSomDeClique();
        Debug.Log("Fechando o jogo...");
        Application.Quit();
    }

    public void BotaoSairNao_Click()
    {
        TocarSomDeClique();
        painelConfirmarSair.SetActive(false);
    }

    public void BotaoVoltar_Opcoes()
    {
        TocarSomDeClique();
        painelOpcoes.SetActive(false);
    }
    
    public void BotaoVoltar_Creditos()
    {
        TocarSomDeClique();
        painelCreditos.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        if (gameMixer != null)
        {
            if (volume <= 0.001f)
            {
                gameMixer.SetFloat("MasterVolume", -80f);
            }
            else
            {
                gameMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            }
        }
        
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    private void TocarSomDeClique()
    {
        // AQUI ESTÁ A CORREÇÃO:
        // Em vez de usar um AudioSource local, chamamos o AudioManager global.
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClickSound();
        }
    }
}