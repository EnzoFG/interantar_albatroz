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

    [Header("Pontuação")]
    public int peixesColetados = 0;

    [Header("Referências de UI")]
    public TextMeshProUGUI textoContadorPeixes;
    public TextMeshProUGUI textoTimer;
    [Space]
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoPeixesFinais;
    public TextMeshProUGUI textoEnergiaGanha;

    [Header("Sons")]
    public AudioClip somColetaPeixe;
    public AudioClip somFimDeJogo;
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
        painelFimDeJogo.SetActive(false);
        AtualizarUI();
    }

    void Update()
    {
        if (jogoAcabou)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(nomeCenaPrincipal);
            }
            return;
        }

        tempoDeJogo -= Time.deltaTime;
        textoTimer.SetText("Tempo: " + tempoDeJogo.ToString("F0"));

        if (tempoDeJogo <= 0)
        {
            FinalizarJogo();
        }
    }

    public void AdicionarPeixe()
    {
        if (jogoAcabou) return;

        peixesColetados++;
        TocarSomColeta();
        AtualizarUI();
    }

    private void FinalizarJogo()
    {
        jogoAcabou = true;
        textoTimer.SetText("Tempo: 0");

        painelFimDeJogo.SetActive(true);

        if (somFimDeJogo != null)
        {
            audioSource.PlayOneShot(somFimDeJogo);
        }

        int energiaGanha = CalcularEnergia();
        textoPeixesFinais.SetText("Peixes Coletados: " + peixesColetados);
        textoEnergiaGanha.SetText("Energia Ganha: " + energiaGanha);
        
    }

    private int CalcularEnergia()
    {
        if (peixesColetados >= 10)
        {
            return 2;
        }
        else if (peixesColetados >= 5)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void TocarSomColeta()
    {
        if (somColetaPeixe != null) audioSource.PlayOneShot(somColetaPeixe);
    }
    
    private void AtualizarUI()
    {
        if (textoContadorPeixes != null) textoContadorPeixes.SetText("Peixes: " + peixesColetados);
    }
}