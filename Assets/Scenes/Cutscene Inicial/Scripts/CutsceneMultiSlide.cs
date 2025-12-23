using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SlideData
{
    public Sprite imagemFundo;
    [TextArea(3, 5)]
    public string[] frases;
}

public class CutsceneMultiSlide : MonoBehaviour
{
    [Header("Referências")]
    public Image componenteImagem;
    public TextMeshProUGUI campoTexto;
    public CanvasGroup painelFade;

    [Header("Conteúdo da Cutscene")]
    public SlideData[] slides;
    public string nomeProximaCena = "MapaMundi";

    [Header("Configurações")]
    public float velocidadeDigitacao = 0.04f;
    public float zoomKenBurns = 0.005f;

    private int slideAtual = 0;
    private int fraseAtual = 0;
    private bool estaDigitando = false;
    private bool podeAvancar = false;

    void Start()
    {
        campoTexto.text = "";
        painelFade.alpha = 1;
        StartCoroutine(ExecutarFluxo());
    }

    void Update()
    {
        if (componenteImagem.sprite != null)
            componenteImagem.transform.localScale += Vector3.one * zoomKenBurns * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (estaDigitando) FinalizarTextoImediatamente();
            else if (podeAvancar) AvancarCena();
        }
    }

    IEnumerator ExecutarFluxo()
    {
        ConfigurarSlide(0);
        
        yield return StartCoroutine(Fade(0)); 
        StartCoroutine(DigitarFrase());
    }

    void ConfigurarSlide(int index)
    {
        componenteImagem.sprite = slides[index].imagemFundo;
        componenteImagem.transform.localScale = Vector3.one;
        fraseAtual = 0;
    }

    IEnumerator DigitarFrase()
    {
        estaDigitando = true;
        podeAvancar = false;
        campoTexto.text = "";

        string textoParaEscrever = slides[slideAtual].frases[fraseAtual];

        foreach (char letra in textoParaEscrever.ToCharArray())
        {
            campoTexto.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao);
            if (!estaDigitando) break;
        }

        estaDigitando = false;
        podeAvancar = true;
    }

    void AvancarCena()
    {
        fraseAtual++;

        if (fraseAtual < slides[slideAtual].frases.Length)
        {
            StartCoroutine(DigitarFrase());
        }
        else
        {
            slideAtual++;
            if (slideAtual < slides.Length)
            {
                StartCoroutine(TrocarSlide());
            }
            else
            {
                SceneManager.LoadScene(nomeProximaCena);
            }
        }
    }

    IEnumerator TrocarSlide()
    {
        podeAvancar = false;
        yield return StartCoroutine(Fade(1));
        ConfigurarSlide(slideAtual);
        yield return StartCoroutine(Fade(0));
        StartCoroutine(DigitarFrase());
    }

    IEnumerator Fade(float alvo)
    {
        float tempo = 0;
        float inicial = painelFade.alpha;
        while (tempo < 1f)
        {
            tempo += Time.deltaTime;
            painelFade.alpha = Mathf.Lerp(inicial, alvo, tempo);
            yield return null;
        }
    }

    void FinalizarTextoImediatamente()
    {
        estaDigitando = false;
        campoTexto.text = slides[slideAtual].frases[fraseAtual];
    }
}