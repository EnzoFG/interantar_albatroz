using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelDePausa;
    public GameObject painelDeOpcoes;
    public GameObject painelConfirmarSair; // <-- NOVA REFERÊNCIA

    [Header("Controle de Volume")]
    public AudioMixer gameMixer;
    public Slider volumeSlider;

    private bool estaPausado = false;

    void Start()
    {
        // Garante que todos os painéis comecem desativados
        painelDePausa.SetActive(false);
        painelDeOpcoes.SetActive(false);
        painelConfirmarSair.SetActive(false); // <-- NOVO

        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = savedVolume;
            AplicarVolume(savedVolume);
        }
    }

    // --- CONTROLE PRINCIPAL DE PAUSA ---

    public void AlternarPausa()
    {
        AudioManager.instance.PlayClickSound();
        estaPausado = !estaPausado;

        if (estaPausado)
        {
            Time.timeScale = 0f;
            painelDePausa.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            painelDePausa.SetActive(false);
            painelDeOpcoes.SetActive(false);
            painelConfirmarSair.SetActive(false); // Garante que tudo feche
        }
    }

    // --- NAVEGAÇÃO ENTRE PAINÉIS ---

    public void AbrirPainelOpcoes()
    {
        AudioManager.instance.PlayClickSound();
        painelDePausa.SetActive(false);
        painelDeOpcoes.SetActive(true);
    }

    public void FecharPainelOpcoes()
    {
        AudioManager.instance.PlayClickSound();
        painelDeOpcoes.SetActive(false);
        painelDePausa.SetActive(true);
    }

    // --- FUNÇÕES DOS BOTÕES DE SAIR (MODIFICADO) ---

    // Agora esta função abre o pop-up de confirmação
    public void PedirConfirmacaoParaSair()
    {
        AudioManager.instance.PlayClickSound();
        painelConfirmarSair.SetActive(true); // Abre o painel de confirmação
        painelDePausa.SetActive(false);      // Esconde o painel de pausa anterior
    }

    // Esta função será chamada pelo botão "Não"
    public void CancelarSaida()
    {
        AudioManager.instance.PlayClickSound();
        painelConfirmarSair.SetActive(false); // Fecha o pop-up
        painelDePausa.SetActive(true);        // Volta para o menu de pausa
    }

    // Esta função será chamada pelo botão "Sim"
    public void ConfirmarSaida()
    {
        AudioManager.instance.PlayClickSound();
        Time.timeScale = 1f;
        Application.Quit();
    }

    // --- LÓGICA DE VOLUME ---
    // (Nenhuma mudança aqui)
    public void MudarVolume(float novoVolume)
    {
        AplicarVolume(novoVolume);
    }

    private void AplicarVolume(float volume)
    {
        if (gameMixer != null)
        {
            float dbVolume = volume > 0.001f ? Mathf.Log10(volume) * 20 : -80f;
            gameMixer.SetFloat("MasterVolume", dbVolume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }
}