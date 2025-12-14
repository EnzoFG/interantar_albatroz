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
    
    [Header("UI Fim de Jogo")]
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoResultadoPeixes;
    public TextMeshProUGUI textoResultadoEnergia;

    [Header("Configuração de Saída e Energia")]
    [Tooltip("Nome da cena para onde ir ao clicar em Continuar")]
    public string nomeDaCenaDeSaida = "ViagemAlbatroz"; // <--- Ajustado
    
    [Tooltip("Quantos peixes são necessários para ganhar 1 de energia")]
    public int peixesParaUmaEnergia = 3; 

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
        painelFimDeJogo.SetActive(false); 
        AtualizarUI();

        if (musicAudioSource != null && backgroundMusicClip != null)
        {
            musicAudioSource.clip = backgroundMusicClip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }

        // Teste de conexão (padrão dos outros minigames)
        if (PlayerPrefs.HasKey("EnergiaPlayer"))
        {
            Debug.Log("BARCO START: Energia recebida: " + PlayerPrefs.GetInt("EnergiaPlayer"));
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

    void TerminarJogo()
    {
        jogoRodando = false;
        Time.timeScale = 0f; 
        if(musicAudioSource) musicAudioSource.Stop(); 

        // 1. Calcula a Energia Ganha
        int energiaGanha = 0;
        if (peixesParaUmaEnergia > 0)
        {
            energiaGanha = peixesPegos / peixesParaUmaEnergia;
        }

        // --- SISTEMA DE INTEGRAÇÃO DE ENERGIA (NOVO) ---
        
        // 2. Lê a energia anterior (Usa 5 se der erro de leitura)
        int energiaAnterior = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        
        // 3. Soma
        int energiaFinal = energiaAnterior + energiaGanha;
        
        // 4. Limita (0 a 10)
        if (energiaFinal > 10) energiaFinal = 10;
        if (energiaFinal < 0) energiaFinal = 0;

        // 5. Salva na chave correta
        PlayerPrefs.SetInt("EnergiaPlayer", energiaFinal);
        PlayerPrefs.Save();
        
        Debug.Log($"BARCO FIM: Tinha {energiaAnterior}. Ganhou {energiaGanha}. Ficou com {energiaFinal}.");
        // -----------------------------------------------

        // Atualiza UI
        textoResultadoPeixes.text = $"Peixes Coletados: {peixesPegos}";
        
        // Formata o texto para ficar bonito (ex: "+1")
        if (energiaGanha > 0) textoResultadoEnergia.text = $"Energia Ganha: +{energiaGanha}";
        else textoResultadoEnergia.text = $"Energia Ganha: {energiaGanha}";

        painelFimDeJogo.SetActive(true);
    }

    public void VoltarParaCenaPrincipal()
    {
        Time.timeScale = 1f; 
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