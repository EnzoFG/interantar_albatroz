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
    [Tooltip("A altura Y onde estão as bandeiras/rede")]
    public float alturaDasBandeirasY = 0f; 
    [Tooltip("Distância a partir das bandeiras onde o risco chega a zero")]
    public float distanciaDeSeguranca = 3f; 
    [Tooltip("A chance máxima de morrer se o peixe nascer EXATAMENTE em cima da bandeira (0-100)")]
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

    [Header("Configuração de Saída e Energia")]
    public string nomeDaCenaDeSaida = "MenuInicial";
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
        
        // Garante que os painéis comecem fechados
        if(painelFimDeJogo != null) painelFimDeJogo.SetActive(false);
        if(painelGameOverRede != null) painelGameOverRede.SetActive(false);
        
        AtualizarUI();

        // Toca a música com segurança
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

    // --- FUNÇÃO CHAMADA PELA AVE (COM RISCO) ---
    public void TentarRegistrarPeixe(float chanceDeMorteDoPeixe)
    {
        if (!jogoRodando) return;

        // Sorteio do risco
        float sorteio = Random.Range(0f, 100f);

        if (sorteio < chanceDeMorteDoPeixe)
        {
            GameOverMorteNaRede();
        }
        else
        {
            // Sucesso
            peixesPegos++;
            AtualizarUI();
            if (sfxAudioSource != null && catchFishSoundClip != null)
            {
                sfxAudioSource.PlayOneShot(catchFishSoundClip);
            }
        }
    }

    // --- GAME OVER POR MORTE (REDE) ---
    void GameOverMorteNaRede()
    {
        jogoRodando = false;
        Time.timeScale = 0f;

        // BLINDAGEM: Só para a música se o objeto de áudio ainda existir
        if(musicAudioSource != null && musicAudioSource.gameObject != null) 
        {
            musicAudioSource.Stop();
        }

        Debug.Log("GAME OVER: A ave ficou presa na rede!");

        if (painelGameOverRede != null) painelGameOverRede.SetActive(true);
    }

    // --- FIM DE JOGO POR TEMPO (VITÓRIA) ---
    void TerminarJogoVitoria()
    {
        jogoRodando = false;
        Time.timeScale = 0f; 
        
        // BLINDAGEM: Só para a música se o objeto de áudio ainda existir
        if(musicAudioSource != null && musicAudioSource.gameObject != null) 
        {
            musicAudioSource.Stop();
        }

        // Cálculo de Energia
        int energiaGanha = 0;
        if (peixesParaUmaEnergia > 0) energiaGanha = peixesPegos / peixesParaUmaEnergia;

        // Atualiza UI
        if(textoResultadoPeixes != null) textoResultadoPeixes.text = $"Total de Peixes: {peixesPegos}";
        if(textoResultadoEnergia != null) textoResultadoEnergia.text = $"Parabéns!\nGanhou +{energiaGanha} de Energia.";

        // Salva Energia
        int energiaTotal = PlayerPrefs.GetInt("EnergiaTotalGlobal", 0);
        PlayerPrefs.SetInt("EnergiaTotalGlobal", energiaTotal + energiaGanha);

        if(painelFimDeJogo != null) painelFimDeJogo.SetActive(true);
    }

    // --- FUNÇÃO DO BOTÃO SAIR (VERSÃO HARD RESET) ---
    public void VoltarParaCenaPrincipal()
    {
        // 1. Garante que o jogo despause
        Time.timeScale = 1f;

        // 2. O TRUQUE DO RESET:
        // Se o AudioManager estiver vivo, nós o destruímos explicitamente.
        // Isso remove qualquer referência "fantasma" que ele esteja segurando.
        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
        }

        // 3. Carrega a cena do Menu
        // Quando o Menu carregar, ele vai criar um NOVO AudioManager limpo.
        SceneManager.LoadScene(nomeDaCenaDeSaida);
    }

    // --- AUXILIARES DE UI ---
    void AtualizarUI() 
    { 
        if(textoPeixes != null) textoPeixes.text = $"Peixes: {peixesPegos}"; 
    }
    
    void AtualizarUITempo() 
    { 
        int seg = Mathf.CeilToInt(tempoRestante); 
        if(textoTempo != null) textoTempo.text = $"Tempo: {seg}"; 
    }

    // --- LÓGICA DO SPAWNER ---
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
            
            // Busca o script PeixePesqueiro (específico deste minigame)
            PeixePesqueiro controller = novoPeixe.GetComponent<PeixePesqueiro>();
            
            if (controller != null) 
            {
                controller.tempoDeVida = tempoDeVidaDoPeixe;

                // Cálculo do Risco
                float distanciaAteBandeira = Mathf.Abs(posicao.y - alturaDasBandeirasY);
                float fatorDeRisco = 1f - Mathf.Clamp01(distanciaAteBandeira / distanciaDeSeguranca);
                controller.chanceDeMorte = fatorDeRisco * riscoMaximoPorcentagem;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Desenha a área de spawn (Ciano)
        Gizmos.color = Color.cyan;
        Vector3 centroGlobal = (Application.isPlaying) ? (Vector3)centroDaArea : transform.position + (Vector3)centroDaArea;
        Gizmos.DrawWireCube(centroGlobal, tamanhoDaArea);

        // Desenha a LINHA DAS BANDEIRAS (Vermelho)
        Gizmos.color = Color.red;
        Vector3 linhaBandeira = new Vector3(centroGlobal.x, alturaDasBandeirasY, 0);
        Gizmos.DrawLine(linhaBandeira + Vector3.left * 5, linhaBandeira + Vector3.right * 5);
        
        // Desenha a ÁREA DE PERIGO (Amarelo transparente)
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(linhaBandeira, new Vector3(10, distanciaDeSeguranca * 2, 0));
    }
}