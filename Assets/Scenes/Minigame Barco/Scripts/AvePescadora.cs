using UnityEngine;

public class AvePescadora : MonoBehaviour
{
    public static AvePescadora instance; // Singleton para o peixe encontrar a ave facil

    [Header("Configurações")]
    public float velocidadeVoo = 8f;
    
    [Header("Referências Visuais")]
    public Animator animator; // Referência ao componente que controla a animação
    public SpriteRenderer spriteRenderer; // Para virar a ave (flip) se necessário

    private Vector3 posicaoInicial; // Onde fica o barco
    private Transform alvoPeixe;    // O peixe que estamos perseguindo
    private bool estaVoando = false;
    private bool voltandoParaCasa = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        posicaoInicial = transform.position; // Grava a posição do barco
    }

    void Update()
    {
        if (estaVoando)
        {
            if (!voltandoParaCasa)
            {
                // --- INDO ATÉ O PEIXE ---
                if (alvoPeixe != null)
                {
                    MoverPara(alvoPeixe.position);

                    // Checa se chegou bem pertinho do peixe
                    if (Vector2.Distance(transform.position, alvoPeixe.position) < 0.1f)
                    {
                        PegarPeixe();
                    }
                }
                else
                {
                    // Se o peixe sumiu (ex: o tempo acabou), volta pra casa
                    voltandoParaCasa = true;
                }
            }
            else
            {
                // --- VOLTANDO PARA O BARCO ---
                MoverPara(posicaoInicial);

                // Checa se chegou no barco
                if (Vector2.Distance(transform.position, posicaoInicial) < 0.1f)
                {
                    PousarNoBarco();
                }
            }
        }
    }

    // Mudamos de 'void' para 'bool' para retornar uma resposta
    public bool DefinirAlvo(Transform peixe)
    {
        // Se já estiver voando, responde "Falso" (Não posso ir)
        if (estaVoando) return false; 

        alvoPeixe = peixe;
        estaVoando = true;
        voltandoParaCasa = false;

        if (animator != null) animator.SetBool("Voando", true);
        
        // Responde "Verdadeiro" (Aceitei o trabalho!)
        return true; 
    }

    void MoverPara(Vector3 destino)
    {
        // Move a ave
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidadeVoo * Time.deltaTime);
        
        // (Opcional) Vira a ave para olhar para onde está indo
        if (destino.x < transform.position.x) spriteRenderer.flipX = true; // Esquerda
        else spriteRenderer.flipX = false; // Direita (depende do seu sprite original)
    }
    void PegarPeixe()
    {
        // 1. Avisa o GameManager para contar o ponto e tocar o som
        if (GameManagerBarco.instance != null)
        {
            GameManagerBarco.instance.RegistrarPeixePego();
        }

        // 2. Destrói o peixe
        if (alvoPeixe != null)
        {
            Destroy(alvoPeixe.gameObject);
        }

        // 3. Começa a voltar
        voltandoParaCasa = true;
    }

    void PousarNoBarco()
    {
        estaVoando = false;
        voltandoParaCasa = false;
        
        // Desativa a animação de voo (volta a ficar sentada)
        if (animator != null) animator.SetBool("Voando", false);
    }
}