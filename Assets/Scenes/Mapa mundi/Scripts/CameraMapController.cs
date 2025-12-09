using UnityEngine;

public class CameraMapController : MonoBehaviour
{
    [Header("Alvos")]
    public Transform aveTransform; // Arraste a sua Ave aqui

    [Header("Configuração: Perto (Zoom In)")]
    public float tamanhoZoomPerto = 5f; // Tamanho da câmera focada na ave

    [Header("Configuração: Longe (Mapa Todo)")]
    public float tamanhoZoomLonge = 20f; // Tamanho para ver o mapa todo
    public Vector3 posicaoCentroMapa; // A posição fixa onde a câmera fica para ver tudo

    [Header("Suavidade")]
    public float velocidadeTroca = 5f; // Quão rápido o zoom acontece

    private Camera cam;
    private bool vendoMapaTodo = false; // Estado atual
    private float tamanhoAlvo;
    private Vector3 posicaoAlvo;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Começa focado na ave
        tamanhoAlvo = tamanhoZoomPerto;
        vendoMapaTodo = false;
    }

    void LateUpdate()
    {
        if (aveTransform == null) return;

        // 1. Define qual é o objetivo (Alvo)
        if (vendoMapaTodo)
        {
            // Estado: Mapa Todo
            tamanhoAlvo = tamanhoZoomLonge;
            
            // --- A CORREÇÃO ESTÁ AQUI ---
            // Em vez de usar "posicaoAlvo = posicaoCentroMapa;" direto,
            // nós criamos um novo vetor usando o X e Y do alvo, mas o Z ATUAL da câmera.
            posicaoAlvo = new Vector3(posicaoCentroMapa.x, posicaoCentroMapa.y, transform.position.z);
        }
        else
        {
            // Estado: Seguindo a Ave
            tamanhoAlvo = tamanhoZoomPerto;
            // Aqui já estava correto, mantendo o Z
            posicaoAlvo = new Vector3(aveTransform.position.x, aveTransform.position.y, transform.position.z);
        }

        // 2. Aplica o movimento suave (Lerp)
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, tamanhoAlvo, Time.deltaTime * velocidadeTroca);
        transform.position = Vector3.Lerp(transform.position, posicaoAlvo, Time.deltaTime * velocidadeTroca);
    }

    // --- FUNÇÃO PÚBLICA PARA O BOTÃO ---
    public void AlternarVisualizacao()
    {
        // Inverte o estado (se era true vira false, e vice-versa)
        vendoMapaTodo = !vendoMapaTodo;
    }
}