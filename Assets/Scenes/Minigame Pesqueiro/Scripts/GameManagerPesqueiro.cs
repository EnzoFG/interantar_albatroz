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

    void GameOverMorteNaRede()
    {
        jogoRodando = false;
        Time.timeScale = 0f;

        if(musicAudioSource != null) musicAudioSource.Stop();

        Debug.Log("GAME OVER: A ave ficou presa na rede!");

        if (painelGameOverRede != null) painelGameOverRede.SetActive(true);
    }

    void TerminarJogoVitoria()
    {
        jogoRodando = false;
        Time.timeScale = 0f; 
        
        if(musicAudioSource != null) musicAudioSource.Stop();

        int energiaGanha = 0;
        if (peixesParaUmaEnergia > 0) energiaGanha = peixesPegos / peixesParaUmaEnergia;

        int energiaAnterior = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        int energiaFinal = energiaAnterior + energiaGanha;
        
        if (energiaFinal > 10) energiaFinal = 10;
        if (energiaFinal < 0) energiaFinal = 0;

        PlayerPrefs.SetInt("EnergiaPlayer", energiaFinal);
        PlayerPrefs.Save();
        
        Debug.Log($"PESQUEIRO VITORIA: Salvo {energiaFinal} de energia.");

        if(textoResultadoPeixes != null) textoResultadoPeixes.text = $"Total de Peixes: {peixesPegos}";
        if(textoResultadoEnergia != null) textoResultadoEnergia.text = $"Energia Ganha: {energiaGanha}";

        if(painelFimDeJogo != null) painelFimDeJogo.SetActive(true);
    }

    public void BotaoContinuarParaMapa()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMapa);
    }

    public void BotaoReiniciarTudo()
    {
        Time.timeScale = 1f;

        PlayerPrefs.DeleteAll();
        Debug.Log("RESET TOTAL: Progresso apagado.");

        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
        }

        SceneManager.LoadScene(nomeCenaMenu);
    }

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