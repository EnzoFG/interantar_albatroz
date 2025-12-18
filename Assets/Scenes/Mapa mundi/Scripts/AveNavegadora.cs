using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class AveNavegadora : MonoBehaviour
{
    [Header("Configuração")]
    public float velocidade = 2f;
    public PontoMapa pontoAtual; 
    
    [Header("Referências de UI")]
    public GameObject popupDecisao;
    public PainelMinigame painelMinigame;
    public GameObject painelVitoria; 

    [Header("Configuração de Fim de Jogo")]
    public string nomeCenaMenu = "MenuInicial"; 

    private bool estaAndando = false;
    private PontoMapa destinoImediato; 
    private static bool jogoJaComecou = false;

    void Awake()
    {
        if (!jogoJaComecou)
        {
            PlayerPrefs.DeleteKey("UltimoPontoMapa"); 
            PlayerPrefs.DeleteKey("EnergiaPlayer");
            jogoJaComecou = true;
        }
    }

    void Start()
    {
        // --- CORREÇÃO 1: GARANTIA DE TEMPO ---
        // Garante que o jogo não esteja pausado ao voltar de um minigame
        Time.timeScale = 1f;

        // Garante que painéis comecem fechados
        if (painelVitoria != null) painelVitoria.SetActive(false);
        if (!pontoAtual.eBifurcacao && popupDecisao != null) popupDecisao.SetActive(false);
        if (painelMinigame != null) painelMinigame.gameObject.SetActive(false);

        // --- CARREGAMENTO DO SAVE ---
        string nomeUltimoPonto = PlayerPrefs.GetString("UltimoPontoMapa", "");
        if (!string.IsNullOrEmpty(nomeUltimoPonto))
        {
            GameObject objPonto = GameObject.Find(nomeUltimoPonto);
            if (objPonto != null) pontoAtual = objPonto.GetComponent<PontoMapa>();
        }

        if (pontoAtual != null)
        {
            transform.position = pontoAtual.transform.position;
            
            // --- CORREÇÃO 2: LÓGICA DE MOVIMENTO NO INÍCIO ---
            
            if (pontoAtual.ePontoFinal)
            {
                // Se nasceu no final, Vitória
                if (painelVitoria != null) painelVitoria.SetActive(true);
            }
            else if (pontoAtual.eBifurcacao)
            {
                // Se nasceu na bifurcação, espera escolha
                if(popupDecisao != null) popupDecisao.SetActive(true);
            }
            else if (pontoAtual.proximoPonto != null)
            {
                // AQUI ESTAVA O PROBLEMA:
                // Removemos a verificação "!pontoAtual.eMinigame".
                // Se carregou a cena e tem um próximo ponto, ela DEVE andar,
                // mesmo que o ponto atual seja de minigame (pois assume-se que já jogou).
                MoverPara(pontoAtual.proximoPonto);
            }
        }
    }

    void Update()
    {
        if (estaAndando && destinoImediato != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinoImediato.transform.position, velocidade * Time.deltaTime);

            if (Vector3.Distance(transform.position, destinoImediato.transform.position) < 0.05f)
            {
                ChegouNoPonto();
            }
        }

        if (!estaAndando && pontoAtual.eBifurcacao)
        {
            DetectarCliqueEscolha();
        }
    }

    void ChegouNoPonto()
    {
        pontoAtual = destinoImediato; 
        transform.position = pontoAtual.transform.position; 
        
        PlayerPrefs.SetString("UltimoPontoMapa", pontoAtual.name);
        PlayerPrefs.Save();

        // --- AQUI MANTEMOS A LÓGICA DE PARAR ---
        // Quando ela CHEGA andando, aí sim ela para se for minigame.
        
        if (pontoAtual.ePontoFinal)
        {
            estaAndando = false;
            if (painelVitoria != null) painelVitoria.SetActive(true);
        }
        else if (pontoAtual.eMinigame)
        {
            estaAndando = false; // Para para jogar
            if (painelMinigame != null)
            {
                painelMinigame.ConfigurarPainel(pontoAtual.imagemDoPainel, pontoAtual.textoExplicativo, pontoAtual.nomeCenaMinigame);
            }
        }
        else if (pontoAtual.eBifurcacao)
        {
            estaAndando = false; // Para para escolher
            if(popupDecisao != null) popupDecisao.SetActive(true);
        }
        else if (pontoAtual.proximoPonto != null)
        {
            MoverPara(pontoAtual.proximoPonto); // Continua andando
        }
        else
        {
            estaAndando = false; 
        }
    }

    public void BotaoReiniciarJogo()
    {
        Time.timeScale = 1f;
        PlayerPrefs.DeleteAll();

        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
        }

        SceneManager.LoadScene(nomeCenaMenu);
    }

    void MoverPara(PontoMapa alvo)
    {
        destinoImediato = alvo;
        Vector3 direcao = alvo.transform.position - transform.position;
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo + 180);
        GetComponent<SpriteRenderer>().flipX = false; 
        estaAndando = true;
    }

    void DetectarCliqueEscolha()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Vector2 posicaoMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(posicaoMouse, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider == pontoAtual.iconeClicavelCima) ConfirmarEscolha(pontoAtual.primeiroPontoCaminhoCima);
                else if (hit.collider == pontoAtual.iconeClicavelBaixo) ConfirmarEscolha(pontoAtual.primeiroPontoCaminhoBaixo);
            }
        }
    }

    void ConfirmarEscolha(PontoMapa primeiroPontoDoCaminho)
    {
        if(popupDecisao != null) popupDecisao.SetActive(false);
        MoverPara(primeiroPontoDoCaminho);
    }
}