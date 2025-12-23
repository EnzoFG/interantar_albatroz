using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PainelMinigame : MonoBehaviour
{
    [Header("Elementos da UI")]
    public Image imagemDestaque;
    public TextMeshProUGUI textoDescricao;
    
    private string cenaParaCarregar;

    public void ConfigurarPainel(Sprite imagem, string texto, string nomeCena)
    {
        if (imagem != null)
        {
            imagemDestaque.sprite = imagem;
            imagemDestaque.gameObject.SetActive(true);
        }
        else
        {
            imagemDestaque.gameObject.SetActive(false);
        }

        textoDescricao.text = texto;

        cenaParaCarregar = nomeCena;

        gameObject.SetActive(true);
    }

    public void BotaoJogar()
    {
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