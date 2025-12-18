using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Conexões Internas")]
    public GameObject painelTutorial;

    [Header("Configuração por Cena")]
    public bool abrirAoIniciar = true;

    // --- NOVA VARIÁVEL DE CONTROLE ---
    // Sendo static, ela lembra o valor mesmo mudando de cena.
    // Começa false. Na primeira vez que mostra, vira true para sempre (nesta sessão).
    private static bool tutorialJaFoiExibido = false;

    IEnumerator Start()
    {
        painelTutorial.SetActive(false);

        // A LÓGICA MUDOU AQUI:
        // Só abre se estiver marcado para abrir E se ainda NÃO foi exibido.
        if (abrirAoIniciar && !tutorialJaFoiExibido)
        {
            yield return new WaitForEndOfFrame();

            // Abre silencioso (false)
            AbrirTutorial(false);
            
            // Marca que já mostrou. Agora, se sair e voltar da cena, ele valerá true.
            tutorialJaFoiExibido = true;
        }
    }

    // Função para chamar via Botão (mantém o som)
    public void AbrirTutorial()
    {
        AbrirTutorial(true);
    }

    // Função interna com controle de som
    public void AbrirTutorial(bool tocarSom)
    {
        if (tocarSom && AudioManager.instance != null)
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
    
    // Opcional: Se quiser testar o tutorial de novo sem fechar o jogo, 
    // você pode criar um método para resetar essa variável ou apenas reiniciar a Unity.
}