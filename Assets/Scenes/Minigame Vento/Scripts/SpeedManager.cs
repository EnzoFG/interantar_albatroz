using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    public static SpeedManager instance;

    [Header("Velocidade da Ave")]
    public float velocidadeEmKmh;
    public float velocidadeEmUnidades;

    [Header("Configurações")]
    public float velocidadeBaseKmh = 40.0f;
    public float fatorConversaoKmh = 0.25f;
    public float velocidadeMinimaKmh = 10.0f;
    public float velocidadeMaximaKmh = 125.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DefinirVelocidadeEmKmh(velocidadeBaseKmh);
    }

    public void DefinirVelocidadeEmKmh(float novaVelocidadeKmh)
    {
        velocidadeEmKmh = Mathf.Clamp(novaVelocidadeKmh, velocidadeMinimaKmh, velocidadeMaximaKmh);

        velocidadeEmUnidades = velocidadeEmKmh * fatorConversaoKmh;
    }

    public void AlterarVelocidadeEmKmh(float quantidade)
    {
        DefinirVelocidadeEmKmh(velocidadeEmKmh + quantidade);
    }
}