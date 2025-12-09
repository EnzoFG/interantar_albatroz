using UnityEngine;

public class PeixeController : MonoBehaviour
{
    [HideInInspector] 
    public float tempoDeVida = 3f; 

    private bool jaFoiClicado = false; // Para evitar cliques duplos

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnMouseDown()
    {
        if (jaFoiClicado || Time.timeScale == 0) return;

        if (AvePescadora.instance != null)
        {
            // AQUI ESTÁ A CORREÇÃO:
            // Nós "perguntamos" à ave se ela pode vir.
            // Só entramos no 'if' se ela retornar 'true'.
            bool aveAceitou = AvePescadora.instance.DefinirAlvo(this.transform);

            if (aveAceitou)
            {
                jaFoiClicado = true; // Agora sim, bloqueamos novos cliques
                CancelInvoke();      // E paramos o timer de destruição
            }
            else
            {
                // Se ela disse não (estava ocupada), não fazemos nada.
                // O jogador pode tentar clicar de novo depois.
                Debug.Log("A ave estava ocupada, tente de novo!");
            }
        }
    }
}