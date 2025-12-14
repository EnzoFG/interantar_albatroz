using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuração")]
    public float tempoDeJogo = 45f;
    public string nomeCenaPrincipal = "ViagemAlbatroz"; 
    
    // Variável interna
    private bool jogoAcabou = false;
    // Propriedade pública para o pássaro ler
    public bool JogoAcabou { get { return jogoAcabou; } }

    [Header("Pontuação")]
    public int peixesColetados = 0;

    [Header("UI")]
    public TextMeshProUGUI textoContadorPeixes; 
    public TextMeshProUGUI textoTimer;
    public GameObject painelFimDeJogo;
    public TextMeshProUGUI textoPeixesFinais;
    public TextMeshProUGUI textoEnergiaGanha; // Onde mostra o resultado da energia

    [Header("Sons")]
    public AudioClip somColetaPeixe;
    public AudioClip somFimDeJogo;
    private AudioSource audioSource;

    void Awake() 
    { 
        if (instance == null) instance = this; 
        else Destroy(gameObject); 
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(painelFimDeJogo) painelFimDeJogo.SetActive(false);
        
        AtualizarUI(); 

        // Teste de conexão
        if (PlayerPrefs.HasKey("EnergiaPlayer"))
        {
            int energiaLida = PlayerPrefs.GetInt("EnergiaPlayer");
            Debug.Log("CONEXÃO OK: O Minigame recebeu que o jogador tem " + energiaLida + " de energia.");
        }
        else
        {
            Debug.LogWarning("AVISO: O Minigame não encontrou a chave 'EnergiaPlayer'. Usando valor padrão.");
        }
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
        if(textoTimer) textoTimer.SetText("Tempo: " + tempoDeJogo.ToString("F0"));

        if (tempoDeJogo <= 0) 
        {
            FinalizarJogo();
        }
    }

    public void AdicionarPeixe()
    {
        if (!jogoAcabou) 
        {
            peixesColetados++;
            TocarSomColeta();
            AtualizarUI();
        }
    }

    private void FinalizarJogo()
    {
        jogoAcabou = true;
        
        if(painelFimDeJogo) painelFimDeJogo.SetActive(true);
        if (somFimDeJogo) audioSource.PlayOneShot(somFimDeJogo);

        // --- CÁLCULO DA ENERGIA ---
        int energiaGanha = 0;
        if (peixesColetados >= 8) energiaGanha = 2;
        else if (peixesColetados >= 4) energiaGanha = 1;

        int energiaAnterior = PlayerPrefs.GetInt("EnergiaPlayer", 5);
        int energiaFinal = energiaAnterior + energiaGanha;
        
        if (energiaFinal > 10) energiaFinal = 10;
        if (energiaFinal < 0) energiaFinal = 0;
        
        PlayerPrefs.SetInt("EnergiaPlayer", energiaFinal);
        PlayerPrefs.Save();

        Debug.Log($"RESULTADO: Tinha {energiaAnterior} + Ganhou {energiaGanha} = Ficou com {energiaFinal}");

        // --- ATUALIZAÇÃO DA UI (CORRIGIDA) ---
        if(textoPeixesFinais) textoPeixesFinais.text = "Peixes Coletados: " + peixesColetados;
        
        if (textoEnergiaGanha)
        {
            // Agora mostramos o texto explicativo junto com o número
            if (energiaGanha > 0)
                textoEnergiaGanha.text = "Energia Ganha: " + energiaGanha;
            else
                textoEnergiaGanha.text = "Energia Ganha: " + energiaGanha;
        }
    }

    private void TocarSomColeta()
    {
        if (somColetaPeixe != null) audioSource.PlayOneShot(somColetaPeixe);
    }

    private void AtualizarUI()
    {
        if (textoContadorPeixes != null) 
        {
            textoContadorPeixes.text = "Peixes: " + peixesColetados;
        }
    }
}