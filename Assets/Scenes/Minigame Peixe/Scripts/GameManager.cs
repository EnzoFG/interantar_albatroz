using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuração do Minigame")]
    public float tempoDeJogo = 45f;
    public string nomeCenaPrincipal = "ViagemAlbatroz"; 
    private bool jogoAcabou = false;
    private bool jogoPausado = false;

    [Header("Pontuação")]
    public int peixesColetados = 0;

    [Header("Referências de UI - HUD")]
    public TextMeshProUGUI textoContadorPeixes;
    public TextMeshProUGUI textoTimer;

    [Header("Referências de UI - Fim de Jogo")]
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoPeixesFinais;
    public TextMeshProUGUI textoEnergiaGanha;

    [Header("Referências de UI - Menu Pausa")]
    public GameObject painelMenuPausa;

    [Header("Sons")]
    public AudioClip somColetaPeixe;
    public AudioClip somFimDeJogo;
    public AudioClip somClickBotao;
    private AudioSource audioSource;

    public bool JogoAcabou { get { return jogoAcabou; } }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if(painelFimDeJogo) painelFimDeJogo.SetActive(false);
        if(painelMenuPausa) painelMenuPausa.SetActive(false);
        
        Time.timeScale = 1f; 
        AtualizarUI();
    }

    void Update()
    {
        if (jogoAcabou) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (jogoPausado) FecharMenuPausa();
            else AbrirMenuPausa();
        }

        if (!jogoPausado)
        {
            tempoDeJogo -= Time.deltaTime;
            if(textoTimer) textoTimer.SetText("Tempo: " + tempoDeJogo.ToString("F0"));

            if (tempoDeJogo <= 0)
            {
                FinalizarJogo();
            }
        }
    }

    public void AdicionarPeixe()
    {
        if (jogoAcabou) return;
        peixesColetados++;
        if (somColetaPeixe != null) audioSource.PlayOneShot(somColetaPeixe);
        AtualizarUI();
    }

    private void FinalizarJogo()
    {
        jogoAcabou = true;
        Time.timeScale = 0f; 
        
        if(painelFimDeJogo) painelFimDeJogo.SetActive(true);
        if (somFimDeJogo != null) audioSource.PlayOneShot(somFimDeJogo);

        int ganho = CalcularEnergia();
        
        int energiaAntiga = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        
        int energiaNova = Mathf.Clamp(energiaAntiga + ganho, 0, 10);
        
        PlayerPrefs.SetInt("EnergiaPlayer", energiaNova);
        PlayerPrefs.Save();

        if(textoPeixesFinais) textoPeixesFinais.SetText("Peixes Coletados: " + peixesColetados);
        if(textoEnergiaGanha) textoEnergiaGanha.SetText("Energia Ganha: " + ganho);
        
        Debug.Log($"Jogo Finalizado! Antiga: {energiaAntiga} | Ganhou: {ganho} | Nova Total: {energiaNova}");
    }

    private int CalcularEnergia()
    {
        if (peixesColetados >= 9) return 2;
        if (peixesColetados >= 4) return 1;
        return 0;
    }

    public void VoltarParaCenaPrincipal()
    {
        TocarSomClick();
        Time.timeScale = 1f; 
        SceneManager.LoadScene(nomeCenaPrincipal);
    }

    public void AbrirMenuPausa()
    {
        if (jogoAcabou) return;
        TocarSomClick();
        jogoPausado = true;
        Time.timeScale = 0f;
        if(painelMenuPausa) painelMenuPausa.SetActive(true);
    }

    public void FecharMenuPausa()
    {
        TocarSomClick();
        jogoPausado = false;
        Time.timeScale = 1f;
        if(painelMenuPausa) painelMenuPausa.SetActive(false);
    }

    public void MudarVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SairDoJogo()
    {
        TocarSomClick();
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void TocarSomClick()
    {
        if (somClickBotao != null && audioSource != null) audioSource.PlayOneShot(somClickBotao);
    }

    private void AtualizarUI()
    {
        if (textoContadorPeixes != null) textoContadorPeixes.SetText("Peixes: " + peixesColetados);
    }
}