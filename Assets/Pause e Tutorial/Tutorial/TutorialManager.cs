using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Conexões Internas")]
    public GameObject painelTutorial;

    [Header("Configuração de Sessão")]
    public string idDoTutorial;
    public bool abrirAoIniciar = true;

    [Header("Alvos de Pausa (Opcional)")]
    [Tooltip("Se você quiser pausar objetos específicos sem erro, arraste-os aqui (ex: Ave, Spawners).")]
    public GameObject[] objetosParaCongelar;

    private static List<string> idsVistosNestaSessao = new List<string>();

    IEnumerator Start()
    {
        if (painelTutorial != null)
            painelTutorial.SetActive(false);

        // Verifica se já viu nesta sessão
        if (abrirAoIniciar && !idsVistosNestaSessao.Contains(idDoTutorial))
        {
            // Pequeno atraso para garantir que tudo carregou
            yield return new WaitForSecondsRealtime(0.1f);

            AbrirTutorial(false);
            idsVistosNestaSessao.Add(idDoTutorial);
        }
    }

    public void AbrirTutorialManual() => AbrirTutorial(true);

    public void AbrirTutorial(bool tocarSom)
    {
        if (painelTutorial == null) return;

        if (tocarSom && AudioManager.instance != null)
            AudioManager.instance.PlayClickSound();

        // 1. Ativa o painel
        painelTutorial.SetActive(true);

        // 2. PAUSA TUDO (Autoridade)
        CongelarMundo(true);
    }

    public void FecharTutorial()
    {
        if (painelTutorial == null) return;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayClickSound();

        // 1. DESPAUSA TUDO
        CongelarMundo(false);

        // 2. Esconde o painel
        painelTutorial.SetActive(false);
    }

    private void CongelarMundo(bool pausar)
    {
        // Para o tempo do motor (Física e Animators)
        Time.timeScale = pausar ? 0f : 1f;

        // Se você arrastou objetos no Inspector (como a Ave), 
        // este código desliga o script deles para eles pararem o Update()
        foreach (GameObject obj in objetosParaCongelar)
        {
            if (obj == null) continue;

            // Desativa todos os scripts do objeto (exceto o Renderer para ele continuar visível)
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
            foreach (var s in scripts)
            {
                // Não desativa a si mesmo nem o próprio painel
                if (s != this) s.enabled = !pausar;
            }

            // Para a simulação física do Rigidbody
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = !pausar;
        }
    }
}