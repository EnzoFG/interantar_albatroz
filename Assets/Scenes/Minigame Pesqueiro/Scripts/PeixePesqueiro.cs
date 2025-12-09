using UnityEngine;

public class PeixePesqueiro : MonoBehaviour
{
    [HideInInspector] 
    public float tempoDeVida = 3f; 

    // A variável de risco exclusiva desse minigame
    [HideInInspector]
    public float chanceDeMorte = 0f;

    private bool jaFoiClicado = false; 

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnMouseDown()
    {
        if (jaFoiClicado || Time.timeScale == 0) return;

        // Procura especificamente pela nova AVE PESQUEIRO
        if (AvePesqueiro.instance != null)
        {
            // Pergunta se a ave pode vir
            bool aveAceitou = AvePesqueiro.instance.DefinirAlvo(this.transform);

            if (aveAceitou)
            {
                jaFoiClicado = true;
                CancelInvoke(); // Para de contar o tempo para sumir
            }
        }
    }
}