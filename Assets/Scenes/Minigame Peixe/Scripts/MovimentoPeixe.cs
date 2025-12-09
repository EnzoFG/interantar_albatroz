using UnityEngine;

public class MovimentoPeixe : MonoBehaviour
{
    public float velocidade = 4f;
    private float limiteXEsquerdo = -15f;

    void Update()
    {
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);

        if (transform.position.x < limiteXEsquerdo)
        {
            Destroy(gameObject);
        }
    }
}