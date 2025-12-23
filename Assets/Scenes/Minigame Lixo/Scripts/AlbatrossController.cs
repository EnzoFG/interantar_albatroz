using UnityEngine;

public class AlbatrossController : MonoBehaviour
{
    private enum State { IDLE, MOVING_TO_TRASH, RETURNING_HOME }
    private State currentState;

    [Header("Configurações de Movimento")]
    public float moveSpeed = 5f;
    private Vector3 homePosition;

    [Header("Detecção de Lixo")]
    [Tooltip("A distância que o lixo precisa estar para atrair a ave")]
    public float detectionRadius = 4f;
    public LayerMask trashLayer;
    private Transform targetTrash;

    [Header("Referências Visuais")]
    public GameObject exclamationMark;

    void Start()
    {
        homePosition = transform.position;
        currentState = State.IDLE;
        exclamationMark.SetActive(false);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.IDLE:
                FindTrashTarget();
                break;

            case State.MOVING_TO_TRASH:
                if (targetTrash != null)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetTrash.position, moveSpeed * Time.deltaTime);
                }
                else
                {
                    ReturnHome();
                }
                break;

            case State.RETURNING_HOME:
                transform.position = Vector2.MoveTowards(transform.position, homePosition, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, homePosition) < 0.1f)
                {
                    currentState = State.IDLE;
                }
                break;
        }
    }

    void FindTrashTarget()
    {
        Collider2D closestTrash = null;
        float minDistance = float.MaxValue;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, trashLayer);

        foreach (var hit in hits)
        {
            TrashItem trashItem = hit.GetComponent<TrashItem>();

            if (trashItem == null || trashItem.isIgnored)
            {
            continue;
            }

            if (hit.transform.position.x < transform.position.x)
            {
                continue;
            }



            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTrash = hit;
            }
    }

    if (closestTrash != null)
    {
        targetTrash = closestTrash.transform;
        currentState = State.MOVING_TO_TRASH;

        GameManagerLixo.instance.PlayAlertSound();
        exclamationMark.SetActive(true);

        targetTrash.GetComponent<TrashItem>().SetTargetable(true);
    }
    }

    public void ReturnHome()
    {
        if (targetTrash != null)
        {
            targetTrash.GetComponent<TrashItem>().SetTargetable(false);
        }
        targetTrash = null;
        currentState = State.RETURNING_HOME;
        exclamationMark.SetActive(false);
    }

    public void CancelTarget()
    {
        targetTrash = null;
        currentState = State.RETURNING_HOME;
        exclamationMark.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

private void OnTriggerEnter2D(Collider2D other)
{
    if (currentState == State.MOVING_TO_TRASH && other.transform == targetTrash)
    {
        GameManagerLixo.instance.ReportTrashEaten();

        ReturnHome();

        Destroy(other.gameObject);
    }
}
}