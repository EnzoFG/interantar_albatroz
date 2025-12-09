using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    private float larguraDoBackground;
    private Vector3 posicaoInicial;

    void Start()
    {
        larguraDoBackground = GetComponent<SpriteRenderer>().bounds.size.x;
        posicaoInicial = transform.position;
    }

    void Update()
    {
        if (SpeedManager.instance == null) return;

        float velocidade = SpeedManager.instance.velocidadeEmUnidades;

        transform.Translate(Vector2.left * velocidade * Time.deltaTime);

        if (transform.position.x < posicaoInicial.x - larguraDoBackground)
        {
            transform.position = posicaoInicial;
        }
    }
}