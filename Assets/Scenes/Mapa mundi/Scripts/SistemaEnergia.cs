using UnityEngine;
using UnityEngine.UI;

public class SistemaEnergia : MonoBehaviour
{
    [Header("Configuração Visual")]
    public Image[] quadrados; 
    public Color corCheio = Color.green;
    public Color corVazio = Color.gray; 

    [Header("Dados")]
    // IMPORTANTE: No Inspector da Unity, mude este número para 5 também!
    public int energiaAtual = 5; 
    private int energiaMaxima = 10;

    void Start()
    {
        // 1. Tenta ler da memória. Se não tiver nada (primeira vez), usa 5.
        energiaAtual = PlayerPrefs.GetInt("EnergiaPlayer", 5);

        // --- A CORREÇÃO DA CONEXÃO ESTÁ AQUI ---
        // Nós FORÇAMOS o salvamento imediato.
        // Isso garante que o valor "5" esteja gravado no disco antes de trocar de cena.
        PlayerPrefs.SetInt("EnergiaPlayer", energiaAtual);
        PlayerPrefs.Save();
        // ---------------------------------------

        Debug.Log("SISTEMA ENERGIA (MAPA): Energia carregada e confirmada na memória: " + energiaAtual);
        
        AtualizarInterface();
    }

    public void ModificarEnergia(int quantidade)
    {
        energiaAtual += quantidade;

        if (energiaAtual > energiaMaxima) energiaAtual = energiaMaxima;
        if (energiaAtual < 0) energiaAtual = 0;

        // Salva sempre que mudar
        PlayerPrefs.SetInt("EnergiaPlayer", energiaAtual);
        PlayerPrefs.Save();

        AtualizarInterface();
    }

    void AtualizarInterface()
    {
        for (int i = 0; i < quadrados.Length; i++)
        {
            if (i < energiaAtual) quadrados[i].color = corCheio;
            else quadrados[i].color = corVazio;
        }
    }
}