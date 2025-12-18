using UnityEngine;
using UnityEngine.UI;

public class ControlePassaro : MonoBehaviour
{
    [Header("Sprites e Animação")]
    // As variáveis de Sprite podem ser mantidas para referência, 
    // mas o controle visual agora é feito pelo Animator.
    private Animator anim; 

    [Header("Movimento")]
    public float velocidadeMergulho = 5f;
    public float velocidadeSubida = 4f;

    [Tooltip("Velocidade com que a ave se move para a direita ao mergulhar.")]
    public float velocidadeHorizontalMergulho = 2f;
    [Tooltip("Velocidade com que a ave volta para a posição X original ao subir.")]
    public float velocidadeRetornoHorizontal = 3f;

    [Header("Fôlego")]
    public Slider barraDeFolego;
    public float folegoMaximo = 10f;
    public float nivelDaAguaY = 0f;
    public float taxaPerdaFolego = 1f;
    public float taxaGanhoFolego = 2f;
    [Tooltip("A porcentagem de fôlego (de 0 a 1) que precisa ser recuperada antes de poder mergulhar de novo.")]
    [Range(0, 1)]
    public float percentualMinimoParaMergulhar = 0.3f;

    [Header("Rotação")]
    [Tooltip("O ângulo final quando o pássaro está totalmente na vertical. -90 para apontar para baixo.")]
    public float anguloMergulhoCompleto = -90f;

    // Variáveis privadas de controle
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 posicaoInicial;
    private bool estaMergulhando = false;
    private float folegoAtual;
    private bool semFolego = false;
    private bool estaRecuperandoFolego = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // Inicializa o Animator

        posicaoInicial = transform.position;
        folegoAtual = folegoMaximo;

        if (barraDeFolego != null)
        {
            barraDeFolego.maxValue = folegoMaximo;
            barraDeFolego.value = folegoAtual;
        }
    }

    void Update()
    {
        // Se o jogo acabou, para o movimento e a animação de mergulho
        if (GameManager.instance.JogoAcabou)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isDiving", false); 
            return;
        }

        GerenciarFolego();
        GerenciarInput();
        AtualizarEstadoPassaro();
        GerenciarRotacao();
    }

    void GerenciarRotacao()
    {
        if (estaMergulhando)
        {
            float progresso = Mathf.InverseLerp(posicaoInicial.y, nivelDaAguaY, transform.position.y);
            float anguloAlvo = Mathf.Lerp(0, anguloMergulhoCompleto, progresso);
            transform.rotation = Quaternion.Euler(0, 0, anguloAlvo);
        }
        else
        {
            // Retorna à posição horizontal imediatamente ao subir
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void AtualizarEstadoPassaro()
    {
        // Atualiza o parâmetro do Animator com base no estado de mergulho
        anim.SetBool("isDiving", estaMergulhando);

        if (estaMergulhando)
        {
            // Movimento de mergulho (Diagonal para baixo e direita)
            rb.linearVelocity = new Vector2(velocidadeHorizontalMergulho, -velocidadeMergulho);
        }
        else
        {
            if (transform.position.y < posicaoInicial.y)
            {
                // Movimento de subida e retorno ao X original
                float velocidadeXRetorno = 0;
                if (Mathf.Abs(transform.position.x - posicaoInicial.x) > 0.1f)
                {
                    float direcaoX = Mathf.Sign(posicaoInicial.x - transform.position.x);
                    velocidadeXRetorno = direcaoX * velocidadeRetornoHorizontal;
                }

                rb.linearVelocity = new Vector2(velocidadeXRetorno, velocidadeSubida);
            }
            else
            {
                // Estabiliza na posição inicial
                rb.linearVelocity = Vector2.zero;
                transform.position = posicaoInicial;
            }
        }
    }

    void GerenciarInput()
    {
        // Permite mergulhar apenas se tiver fôlego e não estiver bloqueado pela recuperação
        if (Input.GetMouseButton(0) && !semFolego && !estaRecuperandoFolego)
        {
            estaMergulhando = true;
        }
        else
        {
            estaMergulhando = false;
        }
    }

    void GerenciarFolego()
    {
        if (transform.position.y < nivelDaAguaY)
        {
            folegoAtual -= taxaPerdaFolego * Time.deltaTime;
        }
        else
        {
            folegoAtual += taxaGanhoFolego * Time.deltaTime;
            if (semFolego) semFolego = false;
        }

        folegoAtual = Mathf.Clamp(folegoAtual, 0, folegoMaximo);

        // Lógica de bloqueio por fôlego esgotado
        if (estaRecuperandoFolego && folegoAtual >= folegoMaximo * percentualMinimoParaMergulhar)
        {
            estaRecuperandoFolego = false;
        }

        if (folegoAtual <= 0)
        {
            semFolego = true;
            estaRecuperandoFolego = true;
        }

        if (barraDeFolego != null)
        {
            barraDeFolego.value = folegoAtual;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Peixe"))
        {
            GameManager.instance.AdicionarPeixe();
            Destroy(other.gameObject);
        }
    }
}