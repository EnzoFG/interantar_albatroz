using UnityEngine;

public class ScrollingItem : MonoBehaviour
{
    [Tooltip("Marque esta caixa se este for o background (para ele repetir infinitamente)")]
    public bool isLoopingBackground = false;

    private float backgroundWidth;
    private Vector3 startPosition;
    private float destroyXPosition = -15f;

    void Start()
    {
        if (isLoopingBackground)
        {
            startPosition = transform.position;
            backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    void Update()
    {
        if (GameManagerLixo.instance == null) return;

        float speed = GameManagerLixo.instance.minigameSpeed;

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (isLoopingBackground)
        {
            if (transform.position.x < startPosition.x - backgroundWidth)
            {
                transform.position = startPosition;
            }
        }
        else
        {
            if (transform.position.x < destroyXPosition)
            {
                Destroy(gameObject);
            }
        }
    }
}