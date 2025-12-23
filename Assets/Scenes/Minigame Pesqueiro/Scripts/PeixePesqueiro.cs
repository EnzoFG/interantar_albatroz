using UnityEngine;

public class PeixePesqueiro : MonoBehaviour
{
    [HideInInspector] 
    public float tempoDeVida = 3f; 

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

        if (AvePesqueiro.instance != null)
        {
            bool aveAceitou = AvePesqueiro.instance.DefinirAlvo(this.transform);

            if (aveAceitou)
            {
                jaFoiClicado = true;
                CancelInvoke();
            }
        }
    }
}