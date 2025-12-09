using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MapPlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidade = 1f; // Velocidade do "Slide" (0 a 1)
    public MapNode noAtual; // Em qual estação a ave está

    [Header("UI do Minigame")]
    public GameObject painelInfo;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoDescricao;
    public Image imagemIcone;
    
    private bool estaViajando = false;

    void Start()
    {
        // Carrega posição (agora buscando MapNode)
        string nomeUltimoPonto = PlayerPrefs.GetString("UltimoPontoMapa", "Ponto_Inicial");
        GameObject objetoPonto = GameObject.Find(nomeUltimoPonto);
        
        if (objetoPonto != null)
        {
            noAtual = objetoPonto.GetComponent<MapNode>();
            transform.position = noAtual.transform.position;
        }
        
        // Verifica o que fazer ao carregar
        StartCoroutine(VerificarProximoPasso());
    }

    // Chamado ao clicar em um Nó de destino (nas bifurcações)
    public void TentarEscolherCaminho(MapNode noClicado)
    {
        if (estaViajando) return;

        // Procura se existe um trilho saindo do nó atual que leva ao nó clicado
        foreach (MapPath trilho in noAtual.caminhosDeSaida)
        {
            if (trilho.pontoChegada.GetComponent<MapNode>() == noClicado)
            {
                StartCoroutine(ViajarPeloTrilho(trilho));
                return;
            }
        }
        Debug.Log("Não há trilho direto para lá!");
    }

    IEnumerator VerificarProximoPasso()
    {
        yield return new WaitForEndOfFrame(); // Espera um frame para segurança

        if (noAtual.temMinigame)
        {
            AbrirPainelMinigame();
        }
        else
        {
            // Lógica Automática
            if (noAtual.caminhosDeSaida.Count == 1)
            {
                // Só tem um caminho, vai sozinho
                StartCoroutine(ViajarPeloTrilho(noAtual.caminhosDeSaida[0]));
            }
            else if (noAtual.caminhosDeSaida.Count > 1)
            {
                Debug.Log("Bifurcação! Escolha um caminho.");
            }
            else
            {
                Debug.Log("Fim da viagem!");
            }
        }
    }

    IEnumerator ViajarPeloTrilho(MapPath trilho)
    {
        estaViajando = true;
        
        // Vira a ave (flip)
        if (trilho.pontoChegada.position.x < transform.position.x)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;

        float progresso = 0f;

        // --- O LOOP DO TREM ---
        // Enquanto o progresso não chegar em 1 (100%)
        while (progresso < 1f)
        {
            progresso += Time.deltaTime * velocidade; // Aumenta o progresso
            
            // Pergunta ao trilho onde fica essa posição na curva
            Vector3 novaPosicao = trilho.GetPosicaoNaCurva(progresso);
            transform.position = novaPosicao;

            yield return null;
        }

        // Chegou
        noAtual = trilho.pontoChegada.GetComponent<MapNode>(); // Atualiza a estação atual
        transform.position = noAtual.transform.position; // Garante posição exata
        estaViajando = false;
        
        PlayerPrefs.SetString("UltimoPontoMapa", noAtual.gameObject.name);

        StartCoroutine(VerificarProximoPasso());
    }

    // --- UI ---
    void AbrirPainelMinigame()
    {
        textoTitulo.text = "Ponto de Interesse";
        textoDescricao.text = noAtual.descricaoMinigame;
        if(noAtual.iconeDoMinigame != null) imagemIcone.sprite = noAtual.iconeDoMinigame;
        imagemIcone.gameObject.SetActive(noAtual.iconeDoMinigame != null);
        painelInfo.SetActive(true);
    }

    public void FecharPainelEContinuar()
    {
        painelInfo.SetActive(false);
        // Gambiarra simples: remove o minigame do objeto na memória 
        // para a ave não abrir o painel de novo imediatamente.
        // (Ao recarregar a cena, volta ao normal, o que é bom)
        noAtual.temMinigame = false; 
        
        StartCoroutine(VerificarProximoPasso());
    }

    public void IrParaMinigame()
    {
        SceneManager.LoadScene(noAtual.nomeDaCenaMinigame);
    }
}