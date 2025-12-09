using UnityEngine;

public class AlbatrossController : MonoBehaviour
{
    // Enum para os estados da ave
    private enum State { IDLE, MOVING_TO_TRASH, RETURNING_HOME }
    private State currentState;

    [Header("Configurações de Movimento")]
    public float moveSpeed = 5f;
    private Vector3 homePosition; // A posição original para onde voltar

    [Header("Detecção de Lixo")]
    [Tooltip("A distância que o lixo precisa estar para atrair a ave")]
    public float detectionRadius = 4f;
    public LayerMask trashLayer; // Para o OverlapCircle só detectar lixo
    private Transform targetTrash;

    [Header("Referências Visuais")]
    public GameObject exclamationMark; // Arraste o objeto "ExclamationMark" aqui

    void Start()
    {
        // Salva a posição inicial
        homePosition = transform.position;
        currentState = State.IDLE;
        exclamationMark.SetActive(false);
    }

    void Update()
    {
        // Executa a lógica baseada no estado atual
        switch (currentState)
        {
            case State.IDLE:
                // Procura por um lixo
                FindTrashTarget();
                break;

            case State.MOVING_TO_TRASH:
                // Se o alvo ainda existir, move-se em direção a ele
                if (targetTrash != null)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetTrash.position, moveSpeed * Time.deltaTime);
                }
                else
                {
                    // Se o alvo foi destruído (ex: saiu da tela), volta para casa
                    ReturnHome();
                }
                break;

            case State.RETURNING_HOME:
                // Move-se de volta para a posição original
                transform.position = Vector2.MoveTowards(transform.position, homePosition, moveSpeed * Time.deltaTime);
                // Se chegou em casa, volta a ficar ocioso
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

        // Pega todos os lixos dentro do raio
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, trashLayer);

        // Analisa cada lixo encontrado
        foreach (var hit in hits)
        {
            // --- NOSSAS NOVAS VERIFICAÇÕES ---

            // 1. Pega o script do lixo
            TrashItem trashItem = hit.GetComponent<TrashItem>();

            // 2. VERIFICA SE O LIXO É IGNORADO
            // Se o lixo não tiver script ou já tiver sido clicado (ignorado), pula para o próximo
            if (trashItem == null || trashItem.isIgnored)
            {
            continue; // Pula este lixo e continua o loop
            }

            // 3. VERIFICA SE O LIXO ESTÁ NA FRENTE
            // Se a posição X do lixo for menor que a posição X da ave, ele está para trás.
            if (hit.transform.position.x < transform.position.x)
            {
                continue; // Pula este lixo e continua o loop
            }

            // --- FIM DAS NOVAS VERIFICAÇÕES ---

            // Se o lixo passou nas verificações, ele é um alvo válido.
            // Agora, verificamos se ele é o mais próximo.
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTrash = hit;
            }
    }

    // Se encontramos um alvo válido (o mais próximo que não está ignorado nem para trás)
    if (closestTrash != null)
    {
        targetTrash = closestTrash.transform;
        currentState = State.MOVING_TO_TRASH;

        GameManagerLixo.instance.PlayAlertSound();
        exclamationMark.SetActive(true);

        targetTrash.GetComponent<TrashItem>().SetTargetable(true);
    }
    }

    // Chamado pelo lixo quando o jogador clica nele
    public void ReturnHome()
    {
        if (targetTrash != null)
        {
            // Diz ao alvo antigo que ele não é mais clicável
            targetTrash.GetComponent<TrashItem>().SetTargetable(false);
        }
        targetTrash = null;
        currentState = State.RETURNING_HOME;
        exclamationMark.SetActive(false);
    }

    // Chamado pelo lixo se ele for destruído
    public void CancelTarget()
    {
        targetTrash = null;
        currentState = State.RETURNING_HOME;
        exclamationMark.SetActive(false);
    }

    // (Opcional) Desenha o raio de detecção no Editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    // ... (Depois da função OnDrawGizmos, no final do script) ...

// CHAMADO QUANDO A AVE TOCA FISICAMENTE EM ALGO
private void OnTriggerEnter2D(Collider2D other)
{
    // 1. Verifica se a ave está indo em direção a um lixo
    // 2. Verifica se o que ela tocou é, de facto, o seu alvo
    if (currentState == State.MOVING_TO_TRASH && other.transform == targetTrash)
    {
        // O jogador falhou! A ave "comeu" o lixo.

        // 1. Avisa ao GameManager para contar a falha e tocar o som
        GameManagerLixo.instance.ReportTrashEaten();

        // 2. Faz a ave voltar para casa
        ReturnHome();

        // 3. Destrói o lixo que foi comido
        Destroy(other.gameObject);
    }
}
}