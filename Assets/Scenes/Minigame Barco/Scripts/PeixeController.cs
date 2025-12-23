using UnityEngine;

public class PeixeController : MonoBehaviour
{
    [HideInInspector] 
    public float tempoDeVida = 3f; 

    private bool jaFoiClicado = false;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnMouseDown()
    {
        if (jaFoiClicado || Time.timeScale == 0) return;

        if (AvePescadora.instance != null)
        {
            bool aveAceitou = AvePescadora.instance.DefinirAlvo(this.transform);

            if (aveAceitou)
            {
                jaFoiClicado = true;
                CancelInvoke();
            }
            else
            {
                Debug.Log("A ave estava ocupada, tente de novo!");
            }
        }
    }
}