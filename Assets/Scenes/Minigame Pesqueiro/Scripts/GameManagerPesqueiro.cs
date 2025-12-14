using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement; 

public class GameManagerPesqueiro : MonoBehaviour
{
    public static GameManagerPesqueiro instance;

    [Header("Configuração Geral")]
    public GameObject peixePrefab; 
    public float tempoDeVidaDoPeixe = 4f; 
    
    [Header("Sistema de Risco (Redes)")]
    public float alturaDasBandeirasY = 0f; 
    public float distanciaDeSeguranca = 3f; 
    public float riscoMaximoPorcentagem = 80f; 

    [Header("UI Game Over (Morte)")]
    public GameObject painelGameOverRede; 

    [Header("UI Principal")]
    public float tempoTotalDoMinigame = 60f;
    public TextMeshProUGUI textoTempo;
    public TextMeshProUGUI textoPeixes;
    
    [Header("UI Vitória (Tempo Acabou)")]
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoResultadoPeixes;
    public TextMeshProUGUI textoResultadoEnergia;

    [Header("Configuração de Cenas")]
    [Tooltip("Nome da cena do Mapa (Para onde vai se ganhar)")]
    public string nomeCenaMapa = "ViagemAlbatroz";
    
    [Tooltip("Nome da cena do Menu Principal (Para onde vai se morrer/reiniciar)")]
    public string nomeCenaMenu = "MenuInicial";

    [Header("Energia")]
    public int peixesParaUmaEnergia = 3;

    private float tempoRestante;
    private int peixesPegos = 0;
    private bool jogoRodando = true;

    [Header("Áudio")]
    public AudioSource musicAudioSource;     
    public AudioSource sfxAudioSource;       
    public AudioClip backgroundMusicClip;  
    public AudioClip catchFishSoundClip;   

    [Header("Spawner")]
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
        
        if(painelFimDeJogo != null) painelFimDeJogo.SetActive(false);
        if(painelGameOverRede != null) painelGameOverRede.SetActive(false);
        
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
            TerminarJogoVitoria();
        }

        // Spawner
        timerSpawner -= Time.deltaTime;
        if (timerSpawner <= 0)
        {
            TentarSpawnarPeixe();
            timerSpawner = spawnInterval;
        }
    }

    public void TentarRegistrarPeixe(float chanceDeMorteDoPeixe)
    {
        if (!jogoRodando) return;

        float sorteio = Random.Range(0f, 100f);

        if (sorteio < chanceDeMorteDoPeixe)
        {
            GameOverMorteNaRede();
        }
        else
        {
            peixesPegos++;
            AtualizarUI();
            if (sfxAudioSource != null && catchFishSoundClip != null)
            {
                sfxAudioSource.PlayOneShot(catchFishSoundClip);
            }
        }
    }

    // --- GAME OVER (MORTE) ---
    void GameOverMorteNaRede()
    {
        jogoRodando = false;
        Time.timeScale = 0f;

        if(musicAudioSource != null) musicAudioSource.Stop();

        Debug.Log("GAME OVER: A ave ficou presa na rede!");

        // Abre o painel de morte. 
        // O botão deste painel deve chamar a função "BotaoReiniciarTudo()"
        if (painelGameOverRede != null) painelGameOverRede.SetActive(true);
    }

    // --- FIM DE JOGO (VITÓRIA / TEMPO ACABOU) ---
    void TerminarJogoVitoria()
    {
        jogoRodando = false;
        Time.timeScale = 0f; 
        
        if(musicAudioSource != null) musicAudioSource.Stop();

        // 1. Cálculo da Energia Ganha
        int energiaGanha = 0;
        if (peixesParaUmaEnergia > 0) energiaGanha = peixesPegos / peixesParaUmaEnergia;

        // 2. Integração com Mapa (Salvar Energia)
        int energiaAnterior = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        int energiaFinal = energiaAnterior + energiaGanha;
        
        if (energiaFinal > 10) energiaFinal = 10;
        if (energiaFinal < 0) energiaFinal = 0;

        PlayerPrefs.SetInt("EnergiaPlayer", energiaFinal);
        PlayerPrefs.Save();
        
        Debug.Log($"PESQUEIRO VITORIA: Salvo {energiaFinal} de energia.");

        // Atualiza UI
        if(textoResultadoPeixes != null) textoResultadoPeixes.text = $"Total de Peixes: {peixesPegos}";
        if(textoResultadoEnergia != null) textoResultadoEnergia.text = $"Parabéns!\nGanhou +{energiaGanha} de Energia.";

        // Abre o painel de vitória.
        // O botão deste painel deve chamar a função "BotaoContinuarParaMapa()"
        if(painelFimDeJogo != null) painelFimDeJogo.SetActive(true);
    }

    // ---------------------------------------------------------------
    // FUNÇÕES PARA OS BOTÕES (ARRAS-TE PARA O ONCLICK DA UI)
    // ---------------------------------------------------------------

    // Use esta função no botão "Continuar" do Painel de VITÓRIA
    public void BotaoContinuarParaMapa()
    {
        Time.timeScale = 1f;
        // Carrega o Mapa Mundi e mantém o progresso salvo
        SceneManager.LoadScene(nomeCenaMapa);
    }

    // Use esta função no botão "Game Over" do Painel de DERROTA (REDE)
    public void BotaoReiniciarTudo()
    {
        Time.timeScale = 1f;

        // 1. APAGA TODO O PROGRESSO (Posição volta pro início, Energia volta pra 5)
        PlayerPrefs.DeleteAll();
        Debug.Log("RESET TOTAL: Progresso apagado.");

        // 2. Destroi o AudioManager para garantir que o som reinicie limpo no Menu
        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
        }

        // 3. Volta para o MENU INICIAL
        SceneManager.LoadScene(nomeCenaMenu);
    }

    // ---------------------------------------------------------------

    void AtualizarUI() 
    { 
        if(textoPeixes != null) textoPeixes.text = $"Peixes: {peixesPegos}"; 
    }
    
    void AtualizarUITempo() 
    { 
        int seg = Mathf.CeilToInt(tempoRestante); 
        if(textoTempo != null) textoTempo.text = $"Tempo: {seg}"; 
    }

    void TentarSpawnarPeixe()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 posicaoAleatoria = CalcularPosicaoAleatoria();
            if (!Physics2D.OverlapCircle(posicaoAleatoria, distanciaMinimaEntrePeixes))
            {
                CriarPeixeComRisco(posicaoAleatoria);
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

    void CriarPeixeComRisco(Vector2 posicao)
    {
        if (peixePrefab != null)
        {
            GameObject novoPeixe = Instantiate(peixePrefab, posicao, Quaternion.identity);
            PeixePesqueiro controller = novoPeixe.GetComponent<PeixePesqueiro>();
            
            if (controller != null) 
            {
                controller.tempoDeVida = tempoDeVidaDoPeixe;
                float distanciaAteBandeira = Mathf.Abs(posicao.y - alturaDasBandeirasY);
                float fatorDeRisco = 1f - Mathf.Clamp01(distanciaAteBandeira / distanciaDeSeguranca);
                controller.chanceDeMorte = fatorDeRisco * riscoMaximoPorcentagem;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 centroGlobal = (Application.isPlaying) ? (Vector3)centroDaArea : transform.position + (Vector3)centroDaArea;
        Gizmos.DrawWireCube(centroGlobal, tamanhoDaArea);

        Gizmos.color = Color.red;
        Vector3 linhaBandeira = new Vector3(centroGlobal.x, alturaDasBandeirasY, 0);
        Gizmos.DrawLine(linhaBandeira + Vector3.left * 5, linhaBandeira + Vector3.right * 5);
        
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(linhaBandeira, new Vector3(10, distanciaDeSeguranca * 2, 0));
    }
}