using UnityEngine;

public class PlayerVerticalFollow : MonoBehaviour
{
    public float moveSpeed = 8f;

    public float minYLimit = -4.5f;
    public float maxYLimit = 4.5f;

    private Rigidbody2D rb;
    private float targetYPosition;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        
        targetYPosition = rb.position.y;
    }

    void Update()
    {
        Vector3 inputScreenPosition = Vector3.zero;
        bool inputActive = false;

        if (Input.touchCount > 0)
        {
            inputScreenPosition = Input.GetTouch(0).position;
            inputActive = true;
        }
        else if (Input.GetMouseButton(0))
        {
            inputScreenPosition = Input.mousePosition;
            inputActive = true;
        }

        if (inputActive)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(inputScreenPosition);
            
            targetYPosition = Mathf.Clamp(worldPosition.y, minYLimit, maxYLimit);
        }
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = rb.position;
        
        Vector2 targetPosition = new Vector2(currentPosition.x, targetYPosition);

        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.fixedDeltaTime);
        
        rb.MovePosition(newPosition);
    }
}