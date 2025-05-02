using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        // Load saved volume (optional)
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = volume;
        AudioListener.volume = volume;

        // Add listener to handle changes
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}