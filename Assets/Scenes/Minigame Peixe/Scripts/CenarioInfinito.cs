using UnityEngine;

public class CenarioInfinito : MonoBehaviour
{
    public float velocidade = 3f;
    public float larguraDaImagem; 

    void Start()
    {
        if (larguraDaImagem <= 0)
        {
            larguraDaImagem = GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    void Update()
    {
        if (GameManager.instance.JogoAcabou || Time.timeScale == 0) return;

        transform.Translate(Vector2.left * velocidade * Time.deltaTime);

        if (transform.position.x <= -larguraDaImagem)
        {
            Vector2 offset = new Vector2(larguraDaImagem * 2, 0);
            transform.position = (Vector2)transform.position + offset;
        }
    }
}