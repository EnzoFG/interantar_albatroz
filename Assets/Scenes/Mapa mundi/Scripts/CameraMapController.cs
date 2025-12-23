using UnityEngine;

public class CameraMapController : MonoBehaviour
{
    [Header("Alvos")]
    public Transform aveTransform;

    [Header("Configuração: Perto (Zoom In)")]
    public float tamanhoZoomPerto = 5f;

    [Header("Configuração: Longe (Mapa Todo)")]
    public float tamanhoZoomLonge = 20f;
    public Vector3 posicaoCentroMapa;

    [Header("Suavidade")]
    public float velocidadeTroca = 5f;

    private Camera cam;
    private bool vendoMapaTodo = false;
    private float tamanhoAlvo;
    private Vector3 posicaoAlvo;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        tamanhoAlvo = tamanhoZoomPerto;
        vendoMapaTodo = false;
    }

    void LateUpdate()
    {
        if (aveTransform == null) return;

        if (vendoMapaTodo)
        {
            tamanhoAlvo = tamanhoZoomLonge;
            
            posicaoAlvo = new Vector3(posicaoCentroMapa.x, posicaoCentroMapa.y, transform.position.z);
        }
        else
        {
            tamanhoAlvo = tamanhoZoomPerto;
            posicaoAlvo = new Vector3(aveTransform.position.x, aveTransform.position.y, transform.position.z);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, tamanhoAlvo, Time.deltaTime * velocidadeTroca);
        transform.position = Vector3.Lerp(transform.position, posicaoAlvo, Time.deltaTime * velocidadeTroca);
    }

    public void AlternarVisualizacao()
    {
        vendoMapaTodo = !vendoMapaTodo;
    }
}