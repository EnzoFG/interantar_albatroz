using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement; // <-- NECESSÁRIO PARA MUDAR DE CENA

public class GameManagerLixo : MonoBehaviour
{
    public static GameManagerLixo instance;

    [Header("Configuração Geral do Minigame")]
    public float minigameSpeed = 5f;

    [Header("Áudio do Minigame")]
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip backgroundMusicClip;
    public AudioClip trashSoundClip;
    public AudioClip alertSoundClip;
    public AudioClip trashEatSoundClip;

    [Header("Configuração do Spawner de Lixo")]
    public GameObject[] trashPrefabs;
    public float spawnIntervalMin = 1.5f;
    public float spawnIntervalMax = 4f;
    public float spawnXPosition = 12f;
    public float spawnYMin = -4.5f;
    public float spawnYMax = 4.5f;
    public float minVerticalDistance = 2.0f;
    public float deadZoneSize = 2.0f; 

    [Header("Estado do Jogo e UI Principal")]
    public float tempoTotalDoMinigame = 60f;
    public TextMeshProUGUI textoTempo;
    public TextMeshProUGUI textoLixosConsumidos;
    
    private float tempoRestante;
    private int lixosConsumidos = 0;
    private bool jogoRodando = true;

    private float timer;
    private float lastSpawnY = 0f;

    // --- SEÇÃO NOVA: FIM DE JOGO ---
    [Header("UI de Fim de Jogo")]
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoResultadoLixos;  // Texto no painel
    public TextMeshProUGUI textoResultadoEnergia; // Texto no painel

    [Header("Configuração de Saída")]
    [Tooltip("Para qual cena voltar ao clicar no botão")]
    public string nomeDaCenaDeSaida = "MenuInicial";
    [Tooltip("Quantos lixos consumidos = -1 de energia")]
    public int lixosPorEnergiaPerdida = 5;
    // --- FIM DA SEÇÃO NOVA ---

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        SetNextSpawnTime();

        if (musicAudioSource != null && backgroundMusicClip != null)
        {
            musicAudioSource.clip = backgroundMusicClip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }

        tempoRestante = tempoTotalDoMinigame;
        lixosConsumidos = 0;
        jogoRodando = true;
        
        // Esconde o painel de fim de jogo ao começar
        painelFimDeJogo.SetActive(false);

        // Ativa a UI principal do jogo
        textoTempo.gameObject.SetActive(true);
        textoLixosConsumidos.gameObject.SetActive(true);

        AtualizarUITempo();
        AtualizarUILixos();
    }

    void Update()
    {
        if (!jogoRodando) return;

        if (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
            AtualizarUITempo();
        }
        else
        {
            tempoRestante = 0;
            AtualizarUITempo(); // Atualiza uma última vez para mostrar "Tempo: 0"
            TerminarJogo();
        }

        // Spawner de Lixo
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnTrash();
            SetNextSpawnTime();
        }
    }

    // --- FUNÇÃO TERMINARJOGO (ATUALIZADA) ---
    void TerminarJogo()
    {
        jogoRodando = false;
        Time.timeScale = 0f; // Pausa o jogo
        musicAudioSource.Stop();

        // Esconde a UI principal do jogo
        textoTempo.gameObject.SetActive(false);
        textoLixosConsumidos.gameObject.SetActive(false);

        // --- LÓGICA DE CÁLCULO E RECOMPENSA ---

        // 1. Calcular Energia
        int energiaPerdida = 0;
        if (lixosPorEnergiaPerdida > 0) // Evita divisão por zero
        {
            // Usa divisão inteira. Ex: 9 lixos / 5 = 1. 10 lixos / 5 = 2.
            energiaPerdida = Mathf.FloorToInt((float)lixosConsumidos / lixosPorEnergiaPerdida);
        }
        
        // A "variável" que será passada para o jogo principal (é um valor negativo)
        int energiaGanhaOuPerdida = -energiaPerdida;

        // 2. Atualizar Textos do Painel
        textoResultadoLixos.text = $"Lixos Consumidos: {lixosConsumidos}";
        textoResultadoEnergia.text = $"Você consumiu {lixosConsumidos} lixos e perdeu {energiaPerdida} de energia.";

        // 3. Salvar no PlayerPrefs (o "fácil de obter")
        // Pega a energia que o jogador já tem
        int energiaTotalAtual = PlayerPrefs.GetInt("EnergiaTotalGlobal", 0);
        // Soma o resultado (que é negativo, então subtrai)
        int novaEnergiaTotal = energiaTotalAtual + energiaGanhaOuPerdida;
        // Salva o novo total
        PlayerPrefs.SetInt("EnergiaTotalGlobal", novaEnergiaTotal);
        
        Debug.Log($"Energia perdida: {energiaPerdida}. Nova energia total salva: {novaEnergiaTotal}");

        // 4. Mostrar o Painel
        painelFimDeJogo.SetActive(true);
    }

    // --- NOVA FUNÇÃO PÚBLICA PARA O BOTÃO ---
    public void VoltarParaCenaPrincipal()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClickSound();
        }
        Time.timeScale = 1f; // Despausa o jogo antes de sair
        SceneManager.LoadScene(nomeDaCenaDeSaida);
    }

    void AtualizarUITempo()
    {
        int segundos = Mathf.CeilToInt(tempoRestante);
        textoTempo.text = $"Tempo: {segundos}";
    }

    void AtualizarUILixos()
    {
        textoLixosConsumidos.text = $"Lixo Consumido: {lixosConsumidos}";
    }

    public void ReportTrashEaten()
    {
        if (!jogoRodando) return; 
        lixosConsumidos++;
        AtualizarUILixos();
        PlayTrashEatSound(); 
    }

    public void PlayTrashEatSound()
    {
        if (sfxAudioSource != null && trashEatSoundClip != null)
        {
            sfxAudioSource.PlayOneShot(trashEatSoundClip);
        }
    }
    
    // ... (Restante das funções de Spawner, Som, etc. sem mudanças) ...
    void SetNextSpawnTime()
    { 
        timer = Random.Range(spawnIntervalMin, spawnIntervalMax); 
    }
    
    public void PlayTrashSound()
    { 
        if (sfxAudioSource != null && trashSoundClip != null) 
        { 
            sfxAudioSource.PlayOneShot(trashSoundClip); 
        } 
    }
    
    public void PlayAlertSound()
    { 
        if (sfxAudioSource != null && alertSoundClip != null) 
        { 
            sfxAudioSource.PlayOneShot(alertSoundClip); 
        } 
    }
    
    void SpawnTrash() 
    { 
        if (trashPrefabs == null || trashPrefabs.Length == 0)
        {
            Debug.LogError("O array 'trashPrefabs' no GameManagerLixo está vazio!");
            return;
        }
        int randomIndex = Random.Range(0, trashPrefabs.Length);
        GameObject prefabToSpawn = trashPrefabs[randomIndex];
        float deadZoneBottom = -deadZoneSize / 2f;
        float deadZoneTop = deadZoneSize / 2f;
        float validBottomZone_Min = spawnYMin;
        float validBottomZone_Max = Mathf.Min(deadZoneBottom, spawnYMax); 
        float validTopZone_Min = Mathf.Max(deadZoneTop, spawnYMin);
        float validTopZone_Max = spawnYMax;
        bool bottomZoneIsValid = validBottomZone_Min < validBottomZone_Max;
        bool topZoneIsValid = validTopZone_Min < validTopZone_Max;
        float spawnY;
        int attempts = 0; 
        do
        {
            if (bottomZoneIsValid && topZoneIsValid)
            {
                if (Random.value > 0.5f) { spawnY = Random.Range(validTopZone_Min, validTopZone_Max); }
                else { spawnY = Random.Range(validBottomZone_Min, validBottomZone_Max); }
            }
            else if (topZoneIsValid) { spawnY = Random.Range(validTopZone_Min, validTopZone_Max); }
            else if (bottomZoneIsValid) { spawnY = Random.Range(validBottomZone_Min, validBottomZone_Max); }
            else
            {
                spawnY = (spawnYMin + spawnYMax) / 2f;
                Debug.LogWarning("O tamanho da Dead Zone é maior que a área de spawn!");
            }
            attempts++;
        }
        while (Mathf.Abs(spawnY - lastSpawnY) < minVerticalDistance && attempts < 10);
        lastSpawnY = spawnY;
        Vector3 spawnPosition = new Vector3(spawnXPosition, spawnY, 0);
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}