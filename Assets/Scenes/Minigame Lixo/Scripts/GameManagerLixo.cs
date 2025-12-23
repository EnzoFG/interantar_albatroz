using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("UI de Fim de Jogo")]
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoResultadoLixos;  
    public TextMeshProUGUI textoResultadoEnergia; 

    [Header("Configuração de Saída")]
    [Tooltip("Para qual cena voltar ao clicar no botão")]
    public string nomeDaCenaDeSaida = "ViagemAlbatroz";
    
    [Tooltip("Quantos lixos consumidos = -1 de energia")]
    public int lixosPorEnergiaPerdida = 5;

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
        
        painelFimDeJogo.SetActive(false);
        textoTempo.gameObject.SetActive(true);
        textoLixosConsumidos.gameObject.SetActive(true);

        AtualizarUITempo();
        AtualizarUILixos();

        if (PlayerPrefs.HasKey("EnergiaPlayer"))
        {
            Debug.Log("LIXO START: Energia recebida: " + PlayerPrefs.GetInt("EnergiaPlayer"));
        }
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
            AtualizarUITempo(); 
            TerminarJogo();
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnTrash();
            SetNextSpawnTime();
        }
    }

    void TerminarJogo()
    {
        jogoRodando = false;
        Time.timeScale = 0f; 
        if(musicAudioSource) musicAudioSource.Stop();

        textoTempo.gameObject.SetActive(false);
        textoLixosConsumidos.gameObject.SetActive(false);

        int energiaPerdida = 0;
        if (lixosPorEnergiaPerdida > 0) 
        {
            energiaPerdida = Mathf.FloorToInt((float)lixosConsumidos / lixosPorEnergiaPerdida);
        }
        
        int energiaAnterior = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        
        int energiaFinal = energiaAnterior - energiaPerdida;
        
        if (energiaFinal > 10) energiaFinal = 10;
        if (energiaFinal < 0) energiaFinal = 0;

        PlayerPrefs.SetInt("EnergiaPlayer", energiaFinal);
        PlayerPrefs.Save();
        
        Debug.Log($"LIXO FIM: Tinha {energiaAnterior}. Perdeu {energiaPerdida}. Ficou com {energiaFinal}.");

        textoResultadoLixos.text = $"Lixos Consumidos: {lixosConsumidos}";
        
        if (energiaPerdida > 0)
        {
            textoResultadoEnergia.text = $"Cuidado! Lixo faz mal.Energia Perdida: -{energiaPerdida}";
        }
        else
        {
            textoResultadoEnergia.text = $"Muito bem! Você evitou o lixo.\nEnergia Mantida.";
        }

        painelFimDeJogo.SetActive(true);
    }

    public void VoltarParaCenaPrincipal()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClickSound();
        }
        Time.timeScale = 1f; 
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
        if (trashPrefabs == null || trashPrefabs.Length == 0) return;
        
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
            }
            attempts++;
        }
        while (Mathf.Abs(spawnY - lastSpawnY) < minVerticalDistance && attempts < 10);
        
        lastSpawnY = spawnY;
        Vector3 spawnPosition = new Vector3(spawnXPosition, spawnY, 0);
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}