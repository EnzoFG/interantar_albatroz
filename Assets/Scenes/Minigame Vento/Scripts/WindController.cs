using UnityEngine;

public class WindController : MonoBehaviour
{
    public enum WindType { Boost, Slow }
    public WindType type = WindType.Boost;

    public float speedChangeAmount = 20f;

    private float destroyXPosition = -15f;

    void Update()
    {
        if (SpeedManager.instance == null) return;

        float velocidadeDoJogo = SpeedManager.instance.velocidadeEmUnidades;

        transform.Translate(Vector2.left * velocidadeDoJogo * Time.deltaTime);

        if (transform.position.x < destroyXPosition)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float amountToSend = (type == WindType.Boost) ? speedChangeAmount : -speedChangeAmount;
            bool wasABoost = (type == WindType.Boost);

            GameManagerVento.instance.ReportWindCollected(amountToSend, wasABoost);

            Destroy(gameObject);
        }
    }
}