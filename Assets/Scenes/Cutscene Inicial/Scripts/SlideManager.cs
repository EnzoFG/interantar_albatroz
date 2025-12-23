using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutsceneInterativa : MonoBehaviour
{
    [Header("Referências")]
    public Image imagemSlide;
    public TextMeshProUGUI campoTexto;
    public CanvasGroup painelFade;

    [Header("Conteúdo")]
    [TextArea(3, 10)]
    public string[] frases;
    public string nomeProximaCena = "MapaMundi";

    [Header("Configurações")]
    public float velocidadeDigitacao = 0.04f;
    public float velocidadeKenBurns = 0.01f;

    private int indexAtual = 0;
    private bool estaDigitando = false;
    private bool podeAvancar = false;

    void Start()
    {
        campoTexto.text = "";
        painelFade.alpha = 1;
        StartCoroutine(IniciarCena());
    }

    void Update()
    {
        if (imagemSlide != null)
            imagemSlide.transform.localScale += Vector3.one * velocidadeKenBurns * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (estaDigitando)
            {
                FinalizarTextoImediatamente();
            }
            else if (podeAvancar)
            {
                AvancarFrase();
            }
        }
    }

    IEnumerator IniciarCena()
    {
        float tempo = 0;
        while (tempo < 1.5f)
        {
            tempo += Time.deltaTime;
            painelFade.alpha = 1 - (tempo / 1.5f);
            yield return null;
        }
        
        StartCoroutine(DigitarFrase());
    }

    IEnumerator DigitarFrase()
    {
        estaDigitando = true;
        podeAvancar = false;
        campoTexto.text = "";

        foreach (char letra in frases[indexAtual].ToCharArray())
        {
            campoTexto.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao);
            if (!estaDigitando) break;
        }

        estaDigitando = false;
        podeAvancar = true;
    }

    void FinalizarTextoImediatamente()
    {
        estaDigitando = false;
        campoTexto.text = frases[indexAtual];
    }

    void AvancarFrase()
    {
        indexAtual++;

        if (indexAtual < frases.Length)
        {
            StartCoroutine(DigitarFrase());
        }
        else
        {
            SceneManager.LoadScene(nomeProximaCena);
        }
    }
}