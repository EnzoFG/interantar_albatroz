using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PainelOpcoesController : MonoBehaviour
{
    public AudioMixer gameMixer;
    public Slider volumeSlider;

    void OnEnable()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        AplicarVolume(savedVolume);
    }

    public void MudarVolume(float novoVolume)
    {
        AplicarVolume(novoVolume);
    }

    public void FecharPainel()
    {
        gameObject.SetActive(false);
    }

    private void AplicarVolume(float volume)
    {
        float dbVolume = volume > 0.001f ? Mathf.Log10(volume) * 20 : -80f;
        gameMixer.SetFloat("MasterVolume", dbVolume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
}