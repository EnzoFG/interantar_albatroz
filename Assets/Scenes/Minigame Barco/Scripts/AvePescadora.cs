using UnityEngine;

public class AvePescadora : MonoBehaviour
{
    public static AvePescadora instance;

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
                    voltandoParaCasa = true;
                }
            }
            else
            {
                MoverPara(posicaoInicial);

                if (Vector2.Distance(transform.position, posicaoInicial) < 0.1f)
                {
                    PousarNoBarco();
                }
            }
        }
    }

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
        
        if (destino.x < transform.position.x) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;
    }
    void PegarPeixe()
    {
        if (GameManagerBarco.instance != null)
        {
            GameManagerBarco.instance.RegistrarPeixePego();
        }

        if (alvoPeixe != null)
        {
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