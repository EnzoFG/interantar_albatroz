using System.Collections;
using UnityEngine;

public class GeradorPeixes : MonoBehaviour
{
    [Header("Configurações do Peixe")]
    public GameObject peixePrefab;

    [Header("Intervalo de Tempo (Aleatório)")]
    [Tooltip("O tempo mínimo de espera entre a criação de peixes.")]
    public float intervaloMinimo = 1.5f;
    [Tooltip("O tempo máximo de espera entre a criação de peixes.")]
    public float intervaloMaximo = 4f;

    [Header("Intervalo de Altura (Aleatório)")]
    [Tooltip("A posição Y mais baixa onde um peixe pode aparecer.")]
    public float alturaMinima = -3f;
    [Tooltip("A posição Y mais alta onde um peixe pode aparecer.")]
    public float alturaMaxima = 1f;


    void Start()
    {
        StartCoroutine(LoopGeradorDePeixes());
    }

    private IEnumerator LoopGeradorDePeixes()
    {
        while (!GameManager.instance.JogoAcabou)
        {
            float tempoDeEspera = Random.Range(intervaloMinimo, intervaloMaximo);
            yield return new WaitForSeconds(tempoDeEspera);

            if (!GameManager.instance.JogoAcabou)
            {
                GerarPeixe();
            }
        }
    }

    void GerarPeixe()
    {
        float posY = Random.Range(alturaMinima, alturaMaxima);
        Vector3 spawnPos = new Vector3(transform.position.x, posY, 0);

        Instantiate(peixePrefab, spawnPos, Quaternion.identity);
    }
}