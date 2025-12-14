using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerVento : MonoBehaviour
{
    public static GameManagerVento instance;

    [Header("Áudio do Minigame")]
    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;
    public AudioClip windCollectSound;
    public AudioClip backgroundMusicClip;

    [Header("Lógica do Jogo")]
    public float tempoTotalDoMinigame = 60f;
    private float tempoRestante;
    private bool jogoRodando = true;

    [Header("Referências da Cena")]
    public PlayerVerticalFollow playerMovement;
    public WindSpawner windSpawner;

    [Header("UI do Minigame")]
    public TextMeshProUGUI textoVelocidade;
    public TextMeshProUGUI textoTempo;
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoVelocidadeMedia;

    [Header("UI de Recompensa")]
    public TextMeshProUGUI textoResultadoEnergia;

    [Header("Configuração de Saída")]
    [Tooltip("O nome exato da cena para onde o jogador voltará")]
    public string nomeDaCenaDeSaida = "ViagemAlbatroz"; // <--- Mudei para o padrão do seu mapa

    [Header("Cálculo da Média")]
    private float somaTotalVelocidade = 0f;
    private float totalQuadrosContados = 0f;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        if (musicAudioSource != null && backgroundMusicClip != null)
        {
            musicAudioSource.clip = backgroundMusicClip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }

        tempoRestante = tempoTotalDoMinigame;
        jogoRodando = true;
        painelFimDeJogo.SetActive(false);
        Time.timeScale = 1f; 

        // --- TESTE DE CONEXÃO (Igual ao outro minigame) ---
        if (PlayerPrefs.HasKey("EnergiaPlayer"))
        {
            int energiaLida = PlayerPrefs.GetInt("EnergiaPlayer");
            Debug.Log("VENTO START: O jogador chegou com " + energiaLida + " de energia.");
        }
        else
        {
            Debug.LogWarning("VENTO AVISO: Chave 'EnergiaPlayer' não encontrada. Usaremos padrão 5 ao salvar.");
        }
    }

    void Update()
    {
        if (!jogoRodando) return;

        if (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;

            if (SpeedManager.instance != null)
            {
                somaTotalVelocidade += SpeedManager.instance.velocidadeEmKmh;
                totalQuadrosContados++;
            }
        }
        else 
        {
            tempoRestante = 0;
            TerminarJogo(); 
        }

        int segundosRestantes = Mathf.CeilToInt(tempoRestante);
        textoTempo.text = $"Tempo: {segundosRestantes}";

        if (SpeedManager.instance != null)
        {
            float velocidadeAtual = SpeedManager.instance.velocidadeEmKmh;
            textoVelocidade.text = $"{velocidadeAtual:F0} km/h";
        }
    }

    void TerminarJogo()
    {
        jogoRodando = false;
        Time.timeScale = 0f; 

        if (playerMovement != null) playerMovement.enabled = false;
        if (windSpawner != null) windSpawner.enabled = false;
        if (musicAudioSource != null) musicAudioSource.Stop();

        int energiaGanha = 0;
        string mensagemResultado = "";

        // Lógica de Pontuação baseada na Velocidade Média
        if (totalQuadrosContados > 0)
        {
            float velocidadeMedia = somaTotalVelocidade / totalQuadrosContados;
            textoVelocidadeMedia.text = $"Sua velocidade média foi: {velocidadeMedia:F1} km/h";

            if (velocidadeMedia < 30f)
            {
                energiaGanha = -1;
                mensagemResultado = "Você perdeu muita velocidade.\nA ave perdeu 1 de energia.";
            }
            else if (velocidadeMedia >= 30f && velocidadeMedia < 50f)
            {
                energiaGanha = 0;
                mensagemResultado = "Você manteve uma velocidade constante.\nEnergia mantida.";
            }
            else
            {
                energiaGanha = 1;
                mensagemResultado = "Incrível! Você aproveitou as correntes de vento!\nGanhou +1 energia!";
            }
        }
        else
        {
            textoVelocidadeMedia.text = "Sem dados de velocidade.";
            energiaGanha = -1;
            mensagemResultado = "Erro no voo. -1 Energia.";
        }

        textoResultadoEnergia.text = mensagemResultado;

        // --- SISTEMA DE INTEGRAÇÃO DE ENERGIA ---
        
        // 1. Lê a energia anterior (Padrão 5 se der erro)
        int energiaAnterior = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        
        // 2. Calcula
        int energiaFinal = energiaAnterior + energiaGanha;
        
        // 3. Trava entre 0 e 10
        if (energiaFinal > 10) energiaFinal = 10;
        if (energiaFinal < 0) energiaFinal = 0;

        // 4. Salva na chave correta
        PlayerPrefs.SetInt("EnergiaPlayer", energiaFinal);
        PlayerPrefs.Save();
        
        Debug.Log($"VENTO FIM: Tinha {energiaAnterior}. Ganhou {energiaGanha}. Ficou com {energiaFinal}.");
        // ----------------------------------------

        painelFimDeJogo.SetActive(true);
    }

    public void ReportWindCollected(float speedChangeAmount, bool isBoost)
    {
        if (sfxAudioSource != null && windCollectSound != null)
        {
            sfxAudioSource.PlayOneShot(windCollectSound);
        }

        if (SpeedManager.instance != null)
        {
            SpeedManager.instance.AlterarVelocidadeEmKmh(speedChangeAmount);
        }
    }

    public void VoltarParaCenaPrincipal()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClickSound();
        }

        Time.timeScale = 1f; // Importante para o mapa não ficar pausado

        SceneManager.LoadScene(nomeDaCenaDeSaida);
    }
}