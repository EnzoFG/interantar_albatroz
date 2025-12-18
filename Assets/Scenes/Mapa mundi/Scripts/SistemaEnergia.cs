using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necessário para trocar de cena

public class SistemaEnergia : MonoBehaviour
{
    [Header("Configuração Visual")]
    public Image[] quadrados; 
    public Color corCheio = Color.green;
    public Color corVazio = Color.gray; 

    [Header("Game Over")]
    public GameObject painelGameOver; // Arraste o seu painel de derrota para cá
    public string nomeCenaMenu = "MenuInicial"; // Nome da cena para onde volta ao reiniciar

    [Header("Dados")]
    public int energiaAtual = 5; 
    private int energiaMaxima = 10;

    void Start()
    {
        // Garante que o painel comece fechado
        if(painelGameOver != null) painelGameOver.SetActive(false);

        // Carrega a energia
        energiaAtual = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        
        // Salva para garantir consistência
        PlayerPrefs.SetInt("EnergiaPlayer", energiaAtual);
        PlayerPrefs.Save();

        AtualizarInterface();
        
        // Verifica se por acaso já começou com 0 (caso raro, mas possível)
        VerificarDerrota();
    }

    public void ModificarEnergia(int quantidade)
    {
        energiaAtual += quantidade;

        if (energiaAtual > energiaMaxima) energiaAtual = energiaMaxima;
        
        // Permite chegar a 0 para disparar o Game Over
        if (energiaAtual < 0) energiaAtual = 0;

        PlayerPrefs.SetInt("EnergiaPlayer", energiaAtual);
        PlayerPrefs.Save();

        AtualizarInterface();
        
        // Verifica se morreu após a mudança
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
        
        // Pausa o jogo para a ave parar de andar
        Time.timeScale = 0f; 
        
        if(painelGameOver != null) painelGameOver.SetActive(true);
    }

    // --- FUNÇÃO PARA O BOTÃO REINICIAR ---
    public void BotaoReiniciarJogo()
    {
        // 1. Despausa o tempo (importante para o menu funcionar)
        Time.timeScale = 1f;

        // 2. Apaga todo o progresso (Reset Total)
        PlayerPrefs.DeleteAll();

        // 3. Reseta o AudioManager (para não bugar o som no menu)
        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
        }

        // 4. Carrega o Menu Inicial
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