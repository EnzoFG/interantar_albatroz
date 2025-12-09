using UnityEngine;

public class ScrollingItem : MonoBehaviour
{
    [Tooltip("Marque esta caixa se este for o background (para ele repetir infinitamente)")]
    public bool isLoopingBackground = false;

    private float backgroundWidth;
    private Vector3 startPosition;
    private float destroyXPosition = -15f; // Posição para auto-destruição

    void Start()
    {
        // Se for um fundo de loop, anota sua posição inicial e largura
        if (isLoopingBackground)
        {
            startPosition = transform.position;
            backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    void Update()
    {
        // Se o GameManagerLixo não existir, não faz nada
        if (GameManagerLixo.instance == null) return;

        // Pega a velocidade de jogo definida no GameManagerLixo
        float speed = GameManagerLixo.instance.minigameSpeed;

        // Move o objeto para a esquerda
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // --- Lógica de Comportamento ---
        if (isLoopingBackground)
        {
            // Se for um fundo, "teleporta" de volta ao início para criar o loop
            if (transform.position.x < startPosition.x - backgroundWidth)
            {
                transform.position = startPosition;
            }
        }
        else
        {
            // Se NÃO for um fundo (ou seja, é um lixo), se destrói ao sair da tela
            if (transform.position.x < destroyXPosition)
            {
                Destroy(gameObject);
            }
        }
    }
}