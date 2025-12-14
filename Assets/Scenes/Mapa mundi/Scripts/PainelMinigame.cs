using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PainelMinigame : MonoBehaviour
{
    [Header("Elementos da UI")]
    public Image imagemDestaque; // Onde vai a arte do Canva
    public TextMeshProUGUI textoDescricao; // Onde vai o texto
    
    private string cenaParaCarregar; // Guarda o nome da cena temporariamente

    // Esta função é chamada pela Ave quando ela chega
    public void ConfigurarPainel(Sprite imagem, string texto, string nomeCena)
    {
        // 1. Atualiza a imagem (se tiver)
        if (imagem != null)
        {
            imagemDestaque.sprite = imagem;
            imagemDestaque.gameObject.SetActive(true);
        }
        else
        {
            // Se esqueceu de por imagem, esconde o slot
            imagemDestaque.gameObject.SetActive(false);
        }

        // 2. Atualiza o texto
        textoDescricao.text = texto;

        // 3. Guarda o nome da cena para usar no botão
        cenaParaCarregar = nomeCena;

        // 4. Mostra o painel
        gameObject.SetActive(true);
    }

    // Função para o Botão "Continuar"
    public void BotaoJogar()
    {
        // Antes de sair, salvamos onde a ave parou!
        // (Isso será tratado no script da Ave, mas aqui carregamos a cena)
        if (!string.IsNullOrEmpty(cenaParaCarregar))
        {
            SceneManager.LoadScene(cenaParaCarregar);
        }
        else
        {
            Debug.LogError("Nome da cena do minigame não foi configurado no PontoMapa!");
        }
    }
}