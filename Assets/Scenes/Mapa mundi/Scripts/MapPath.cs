using UnityEngine;

[ExecuteInEditMode] // Permite ver a linha enquanto edita, sem dar Play!
public class MapPath : MonoBehaviour
{
    [Header("Conexão")]
    public Transform pontoSaida;   // De onde sai (Nó A)
    public Transform pontoChegada; // Para onde vai (Nó B)
    
    [Header("Curvatura")]
    public Transform pontoCurva;   // O "imã" que define a curva

    [Header("Visual")]
    public int resolucaoDaLinha = 20; // Qualidade do desenho
    private LineRenderer lineRenderer;

    void OnEnable()
    {
        // Adiciona um LineRenderer automaticamente se não tiver
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null) 
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.widthMultiplier = 0.15f; // Espessura da linha
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
        }
    }

    void Update()
    {
        // Desenha a linha na cena o tempo todo
        DesenharCaminho();
    }

    // --- A MÁGICA DA MATEMÁTICA BÉZIER ---
    // t vai de 0 (inicio) a 1 (fim)
    public Vector3 GetPosicaoNaCurva(float t)
    {
        if(pontoSaida == null || pontoChegada == null || pontoCurva == null)
            return Vector3.zero;

        // Fórmula de Bézier Quadrática
        // (1-t)² * P0 + 2(1-t)t * P1 + t² * P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = (uu * pontoSaida.position) + (2 * u * t * pontoCurva.position) + (tt * pontoChegada.position);
        return p;
    }

    void DesenharCaminho()
    {
        if (lineRenderer == null || pontoSaida == null || pontoChegada == null || pontoCurva == null) return;

        lineRenderer.positionCount = resolucaoDaLinha;
        for (int i = 0; i < resolucaoDaLinha; i++)
        {
            float t = i / (float)(resolucaoDaLinha - 1);
            Vector3 pos = GetPosicaoNaCurva(t);
            lineRenderer.SetPosition(i, pos);
        }
    }
}