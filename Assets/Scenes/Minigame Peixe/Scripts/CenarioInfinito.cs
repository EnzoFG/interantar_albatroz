using UnityEngine;

public class CenarioInfinito : MonoBehaviour
{
    public float velocidade = 3f;
    public float larguraDaImagem; 

    void Start()
    {
        // Se não preencheu a largura, ele tenta medir o Sprite automaticamente
        if (larguraDaImagem <= 0)
        {
            larguraDaImagem = GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    void Update()
    {
        if (GameManager.instance.JogoAcabou || Time.timeScale == 0) return;

        // Move para a esquerda
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);

        // LOGICA DE REPOSICIONAMENTO:
        // Se a onda sair totalmente da tela (pelo lado esquerdo do centro)
        if (transform.position.x <= -larguraDaImagem)
        {
            // Em vez de resetar baseado na própria posição, 
            // nós a jogamos para a frente da outra onda.
            // Somamos 2x a largura porque temos 2 imagens em fila.
            Vector2 offset = new Vector2(larguraDaImagem * 2, 0);
            transform.position = (Vector2)transform.position + offset;
        }
    }
}