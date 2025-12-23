using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SistemaEnergia : MonoBehaviour
{
    [Header("Configuração Visual")]
    public Image[] quadrados; 
    public Color corCheio = Color.green;
    public Color corVazio = Color.gray; 

    [Header("Game Over")]
    public GameObject painelGameOver;
    public string nomeCenaMenu = "MenuInicial";

    [Header("Dados")]
    public int energiaAtual = 5; 
    private int energiaMaxima = 10;

    void Start()
    {
        if(painelGameOver != null) painelGameOver.SetActive(false);

        energiaAtual = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        
        PlayerPrefs.SetInt("EnergiaPlayer", energiaAtual);
        PlayerPrefs.Save();

        AtualizarInterface();
        
        VerificarDerrota();
    }

    public void ModificarEnergia(int quantidade)
    {
        energiaAtual += quantidade;

        if (energiaAtual > energiaMaxima) energiaAtual = energiaMaxima;
        
        if (energiaAtual < 0) energiaAtual = 0;

        PlayerPrefs.SetInt("EnergiaPlayer", energiaAtual);
        PlayerPrefs.Save();

        AtualizarInterface();
        
        VerificarDerrota();
    }

    void VerificarDerrota()
    {
        if (energiaAtual <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER: Energia chegou a zero.");
        
        Time.timeScale = 0f; 
        
        if(painelGameOver != null) painelGameOver.SetActive(true);
    }

    public void BotaoReiniciarJogo()
    {
        Time.timeScale = 1f;

        PlayerPrefs.DeleteAll();

        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
        }

        SceneManager.LoadScene(nomeCenaMenu);
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