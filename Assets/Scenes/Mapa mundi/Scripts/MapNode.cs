using UnityEngine;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    [Header("Informações")]
    public string nomeDoPonto = "Estação";
    
    [Header("Trilhos de Saída")]
    // Lista de CAMINHOS que saem daqui, não pontos
    public List<MapPath> caminhosDeSaida; 

    [Header("Minigame (Opcional)")]
    public bool temMinigame = false;
    public string nomeDaCenaMinigame; 
    [TextArea] public string descricaoMinigame; 
    public Sprite iconeDoMinigame; 

    private MapPlayerController playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<MapPlayerController>();
    }

    void OnMouseDown()
    {
        // Se clicar no NÓ, tenta ir para ele
        if (playerController != null)
        {
            playerController.TentarEscolherCaminho(this);
        }
    }
    
    // Ajuda visual para ver onde clicar
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}