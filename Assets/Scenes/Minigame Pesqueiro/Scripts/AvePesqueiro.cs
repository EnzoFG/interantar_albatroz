using UnityEngine;

public class AvePesqueiro : MonoBehaviour
{
    public static AvePesqueiro instance;

    [Header("Configurações")]
    public float velocidadeVoo = 8f;
    
    [Header("Referências Visuais")]
    public Animator animator; 
    public SpriteRenderer spriteRenderer; 

    private Vector3 posicaoInicial; 
    private Transform alvoPeixe;    
    private bool estaVoando = false;
    private bool voltandoParaCasa = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        posicaoInicial = transform.position; 
    }

    void Update()
    {
        if (estaVoando)
        {
            if (!voltandoParaCasa)
            {
                // INDO
                if (alvoPeixe != null)
                {
                    MoverPara(alvoPeixe.position);
                    if (Vector2.Distance(transform.position, alvoPeixe.position) < 0.1f)
                    {
                        PegarPeixe();
                    }
                }
                else
                {
                    voltandoParaCasa = true; // Se o peixe sumiu, volta
                }
            }
            else
            {
                // VOLTANDO
                MoverPara(posicaoInicial);
                if (Vector2.Distance(transform.position, posicaoInicial) < 0.1f)
                {
                    PousarNoBarco();
                }
            }
        }
    }

    // Retorna bool para confirmar para o peixe
    public bool DefinirAlvo(Transform peixe)
    {
        if (estaVoando) return false; 

        alvoPeixe = peixe;
        estaVoando = true;
        voltandoParaCasa = false;

        if (animator != null) animator.SetBool("Voando", true);
        
        return true; 
    }

    void MoverPara(Vector3 destino)
    {
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidadeVoo * Time.deltaTime);
        
        // Espelhamento do sprite
        if (destino.x < transform.position.x) spriteRenderer.flipX = true; 
        else spriteRenderer.flipX = false;
    }

    void PegarPeixe()
    {
        if (alvoPeixe != null)
        {
            // --- AQUI ESTÁ A MUDANÇA PRINCIPAL ---
            // Pega o script NOVO do peixe
            PeixePesqueiro scriptPeixe = alvoPeixe.GetComponent<PeixePesqueiro>();
            
            float risco = 0f;
            if (scriptPeixe != null)
            {
                risco = scriptPeixe.chanceDeMorte;
            }

            // Manda o risco para o Gerente
            if (GameManagerPesqueiro.instance != null)
            {
                GameManagerPesqueiro.instance.TentarRegistrarPeixe(risco);
            }

            Destroy(alvoPeixe.gameObject);
        }

        voltandoParaCasa = true;
    }

    void PousarNoBarco()
    {
        estaVoando = false;
        voltandoParaCasa = false;
        if (animator != null) animator.SetBool("Voando", false);
    }
}