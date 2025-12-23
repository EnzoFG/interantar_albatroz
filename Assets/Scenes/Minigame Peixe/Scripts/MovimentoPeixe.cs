using UnityEngine;

public class MovimentoPeixe : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade com que o peixe cruza a tela da direita para a esquerda.")]
    public float velocidade = 4f; 

    private float limiteXEsquerdo = -15f; 

    void Update()
    {
        if (Time.timeScale == 0) return;

        transform.Translate(Vector2.left * velocidade * Time.deltaTime);

        if (transform.position.x < limiteXEsquerdo)
        {
            Destroy(gameObject);
        }
    }
}