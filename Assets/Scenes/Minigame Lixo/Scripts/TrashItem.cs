using UnityEngine;

public class TrashItem : MonoBehaviour
{
    private bool isTargetable = false; // Flag mais importante: o jogador pode clicar neste lixo?
    public bool isIgnored = false;
    private float destroyXPosition = -15f;
    private AlbatrossController albatross; // Referência ao jogador

    void Start()
    {
        // Encontra o Albatroz na cena para poder chamá-lo
        // Usamos FindObjectOfType pois só haverá um Albatroz
        albatross = FindFirstObjectByType<AlbatrossController>();
    }

    void Update()
    {
        // 1. Lógica de movimento (copiada do ScrollingItem)
        if (GameManagerLixo.instance == null) return;
        float speed = GameManagerLixo.instance.minigameSpeed;
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 2. Lógica de destruição
        if (transform.position.x < destroyXPosition)
        {
            // Se for destruído enquanto era o alvo, avisa o albatroz
            if (isTargetable && albatross != null)
            {
                albatross.CancelTarget(); // Diz ao albatroz para parar de seguir
            }
            Destroy(gameObject);
        }
    }

    // 3. Detecção de Clique (precisa de um Collider2D no lixo)
    void OnMouseDown()
    {
        // SÓ FUNCIONA se o Albatroz estiver vindo em direção a ESTE lixo
        if (isTargetable)
        {
            // Toca o som de "clique no lixo"
            GameManagerLixo.instance.PlayTrashSound();

            // Avisa ao Albatroz que ele pode voltar para casa
            albatross.ReturnHome();

            // Não é mais clicável
            isTargetable = false;
            isIgnored = true;
        }
        // Se isTargetable == false, nada acontece, como você pediu.
    }

    // 4. Funções públicas chamadas pelo Albatroz
    public void SetTargetable(bool status)
    {
        isTargetable = status;
    }

    // Chamado se o albatroz mudar de alvo
    public void ClearTarget()
    {
        isTargetable = false;
    }
}