using UnityEngine;

public class PontoMapa : MonoBehaviour
{
    [Header("Tipo de Ponto")]
    public bool eBifurcacao = false;
    public bool eMinigame = false;
    public bool ePontoFinal = false;

    [Header("Caminho Padrão")]
    public PontoMapa proximoPonto;

    [Header("Opções da Bifurcação")]
    public PontoMapa primeiroPontoCaminhoCima;
    public PontoMapa primeiroPontoCaminhoBaixo;
    public Collider2D iconeClicavelCima; 
    public Collider2D iconeClicavelBaixo;

    [Header("DADOS DO MINIGAME (Preencher se for Minigame)")]
    public string nomeCenaMinigame;
    
    public Sprite imagemDoPainel; 
    
    [TextArea(3, 10)] 
    public string textoExplicativo;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (proximoPonto != null)
            Gizmos.DrawLine(transform.position, proximoPonto.transform.position);

        if (eBifurcacao)
        {
            Gizmos.color = Color.green;
            if (primeiroPontoCaminhoCima != null)
                Gizmos.DrawLine(transform.position, primeiroPontoCaminhoCima.transform.position);
            Gizmos.color = Color.red;
            if (primeiroPontoCaminhoBaixo != null)
                Gizmos.DrawLine(transform.position, primeiroPontoCaminhoBaixo.transform.position);
        }
        
        if (ePontoFinal)
        {
            Gizmos.color = Color.magenta;
        }
        else if (eMinigame)
        {
            Gizmos.color = Color.cyan;
        }
        else if (eBifurcacao)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}