using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement; 

public class GameManagerBarco : MonoBehaviour
{
    public static GameManagerBarco instance;

    [Header("Configuração Geral")]
    public GameObject peixePrefab; 
    public float tempoDeVidaDoPeixe = 4f; 
    
    [Header("UI Principal")]
    public float tempoTotalDoMinigame = 60f;
    public TextMeshProUGUI textoTempo;
    public TextMeshProUGUI textoPeixes;
    
    // --- NOVO: UI DE FIM DE JOGO ---
    [Header("UI Fim de Jogo")]
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoResultadoPeixes;
    public TextMeshProUGUI textoResultadoEnergia;

    // --- NOVO: CONFIGURAÇÕES DE RECOMPENSA E SAÍDA ---
    [Header("Configuração de Saída e Energia")]
    [Tooltip("Nome da cena para onde ir ao clicar em Continuar")]
    public string nomeDaCenaDeSaida = "MenuInicial";
    
    [Tooltip("Quantos peixes são necessários para ganhar 1 de energia")]
    public int peixesParaUmaEnergia = 3; // Ex: A cada 3 peixes = 1 energia

    private float tempoRestante;
    private int peixesPegos = 0;
    private bool jogoRodando = true;

    [Header("Áudio do Minigame")]
    public AudioSource musicAudioSource;     
    public AudioSource sfxAudioSource;       
    public AudioClip backgroundMusicClip;  
    public AudioClip catchFishSoundClip;   

    [Header("Configuração do Spawner")]
    public float spawnInterval = 2f; 
    public float distanciaMinimaEntrePeixes = 1.5f; 
    public Vector2 tamanhoDaArea = new Vector2(5f, 3f); 
    public Vector2 centroDaArea = new Vector2(0f, 0f); 

    private float timerSpawner;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        tempoRestante = tempoTotalDoMinigame;
        peixesPegos = 0;
        jogoRodando = true;
        painelFimDeJogo.SetActive(false); // Garante que comece fechado
        AtualizarUI();

        if (musicAudioSource != null && backgroundMusicClip != null)
        {
            musicAudioSource.clip = backgroundMusicClip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }

    void Update()
    {
        if (!jogoRodando) return;

        // Timer
        if (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
            AtualizarUITempo();
        }
        else
        {
            tempoRestante = 0;
            AtualizarUITempo();
            TerminarJogo();
        }

        // Spawner
        timerSpawner -= Time.deltaTime;
        if (timerSpawner <= 0)
        {
            TentarSpawnarPeixe();
            timerSpawner = spawnInterval;
        }
    }

    public void RegistrarPeixePego()
    {
        if (!jogoRodando) return;

        peixesPegos++;
        AtualizarUI();
        
        if (sfxAudioSource != null && catchFishSoundClip != null)
        {
            sfxAudioSource.PlayOneShot(catchFishSoundClip);
        }
    }

    // --- LÓGICA DE FIM DE JOGO ---
    void TerminarJogo()
    {
        jogoRodando = false;
        Time.timeScale = 0f; // Pausa o jogo
        musicAudioSource.Stop(); // Para a música

        // 1. Calcula a Energia
        int energiaGanha = 0;
        if (peixesParaUmaEnergia > 0)
        {
            // Divisão inteira (Ex: 7 peixes / 3 = 2 energias)
            energiaGanha = peixesPegos / peixesParaUmaEnergia;
        }

        // 2. Atualiza os Textos do Painel
        textoResultadoPeixes.text = $"Peixes Coletados: {peixesPegos}";
        textoResultadoEnergia.text = $"Energia Ganha: {energiaGanha}";

        // 3. Salva no Banco de Dados Global (PlayerPrefs)
        int energiaTotalAtual = PlayerPrefs.GetInt("EnergiaTotalGlobal", 0);
        int novaEnergiaTotal = energiaTotalAtual + energiaGanha;
        PlayerPrefs.SetInt("EnergiaTotalGlobal", novaEnergiaTotal);

        Debug.Log($"Peixes: {peixesPegos}. Energia Ganha: {energiaGanha}. Total Salvo: {novaEnergiaTotal}");

        // 4. Mostra o Painel
        painelFimDeJogo.SetActive(true);
    }

    // --- FUNÇÃO DO BOTÃO CONTINUAR ---
    public void VoltarParaCenaPrincipal()
    {
        Time.timeScale = 1f; // Despausa antes de sair
        SceneManager.LoadScene(nomeDaCenaDeSaida);
    }

    void AtualizarUI()
    {
        textoPeixes.text = $"Peixes: {peixesPegos}";
    }

    void AtualizarUITempo()
    {
        int segundos = Mathf.CeilToInt(tempoRestante);
        textoTempo.text = $"Tempo: {segundos}";
    }

    void TentarSpawnarPeixe()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 posicaoAleatoria = CalcularPosicaoAleatoria();
            if (!Physics2D.OverlapCircle(posicaoAleatoria, distanciaMinimaEntrePeixes))
            {
                CriarPeixe(posicaoAleatoria);
                return; 
            }
        }
    }

    Vector2 CalcularPosicaoAleatoria()
    {
        float minX = centroDaArea.x - (tamanhoDaArea.x / 2);
        float maxX = centroDaArea.x + (tamanhoDaArea.x / 2);
        float minY = centroDaArea.y - (tamanhoDaArea.y / 2);
        float maxY = centroDaArea.y + (tamanhoDaArea.y / 2);
        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    void CriarPeixe(Vector2 posicao)
    {
        if (peixePrefab != null)
        {
            GameObject novoPeixe = Instantiate(peixePrefab, posicao, Quaternion.identity);
            PeixeController controller = novoPeixe.GetComponent<PeixeController>();
            if (controller != null) controller.tempoDeVida = tempoDeVidaDoPeixe;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 centroGlobal = (Application.isPlaying) ? (Vector3)centroDaArea : transform.position + (Vector3)centroDaArea;
        Gizmos.DrawWireCube(centroGlobal, tamanhoDaArea);
    }
}