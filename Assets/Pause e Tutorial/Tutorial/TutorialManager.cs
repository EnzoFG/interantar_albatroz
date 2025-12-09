using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Conexões Internas")]
    public GameObject painelTutorial;

    [Header("Configuração por Cena")]
    public bool abrirAoIniciar = true;

    IEnumerator Start()
    {
        painelTutorial.SetActive(false);

        if (abrirAoIniciar)
        {
            yield return new WaitForEndOfFrame();

            AbrirTutorial();
        }
    }

    public void AbrirTutorial()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClickSound();
        }

        Time.timeScale = 0f;
        painelTutorial.SetActive(true);
    }

    public void FecharTutorial()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClickSound();
        }

        Time.timeScale = 1f;
        painelTutorial.SetActive(false);
    }
}