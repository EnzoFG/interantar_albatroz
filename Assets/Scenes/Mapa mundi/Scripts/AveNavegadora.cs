using UnityEngine;
using TMPro;

public class AveNavegadora : MonoBehaviour
{
    [Header("Configuração")]
    public float velocidade = 2f;
    public PontoMapa pontoAtual; 
    
    [Header("Referências de UI")]
    public GameObject popupDecisao;
    public PainelMinigame painelMinigame;

    private bool estaAndando = false;
    private PontoMapa destinoImediato; 
    
    // Variável estática para controlar o reset da sessão
    private static bool jogoJaComecou = false;

    void Awake()
    {
        if (!jogoJaComecou)
        {
            // Reseta APENAS a posição e a energia para começar o jogo do zero
            PlayerPrefs.DeleteKey("UltimoPontoMapa"); 
            PlayerPrefs.DeleteKey("EnergiaPlayer");
            
            jogoJaComecou = true;
            Debug.Log("RESET DE SESSÃO: Novo jogo iniciado (Posição e Energia resetadas).");
        }
    }

    void Start()
    {
        // Garante que as UIs comecem fechadas
        if(popupDecisao != null) popupDecisao.SetActive(false);
        if(painelMinigame != null) painelMinigame.gameObject.SetActive(false);

        // 1. Tenta carregar o último ponto salvo
        // (Isso resolve o erro da imagem 1: declaramos a variável apenas uma vez aqui)
        string nomeUltimoPonto = PlayerPrefs.GetString("UltimoPontoMapa", "");
        
        if (!string.IsNullOrEmpty(nomeUltimoPonto))
        {
            GameObject objPonto = GameObject.Find(nomeUltimoPonto);
            if (objPonto != null) pontoAtual = objPonto.GetComponent<PontoMapa>();
        }

        // 2. Posiciona a ave e decide se deve andar
        if (pontoAtual != null)
        {
            transform.position = pontoAtual.transform.position;
            
            // Regra importante: Se carregar o jogo e não estivermos numa encruzilhada, 
            // a ave deve continuar andando automaticamente (simula a volta do minigame)
            if (pontoAtual.eBifurcacao)
            {
                if(popupDecisao != null) popupDecisao.SetActive(true);
            }
            else if (pontoAtual.proximoPonto != null)
            {
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
        
        // Salva o progresso
        PlayerPrefs.SetString("UltimoPontoMapa", pontoAtual.name);
        PlayerPrefs.Save();

        if (pontoAtual.eMinigame)
        {
            estaAndando = false;
            if (painelMinigame != null)
            {
                painelMinigame.ConfigurarPainel(pontoAtual.imagemDoPainel, pontoAtual.textoExplicativo, pontoAtual.nomeCenaMinigame);
            }
        }
        else if (pontoAtual.eBifurcacao)
        {
            estaAndando = false;
            if(popupDecisao != null) popupDecisao.SetActive(true);
        }
        else if (pontoAtual.proximoPonto != null)
        {
            MoverPara(pontoAtual.proximoPonto);
        }
        else
        {
            estaAndando = false; 
        }
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