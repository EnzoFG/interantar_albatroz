using UnityEngine;

public class BotaoAbrePainel : MonoBehaviour
{
    public GameObject painelParaAbrir;

    public void AbrirPainel()
    {
        if (painelParaAbrir != null)
        {
            painelParaAbrir.SetActive(true);
        }
        else
        {
            Debug.LogError("O painel não foi atribuído a este botão!", this);
        }
    }
}