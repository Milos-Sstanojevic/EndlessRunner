using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GlobalAudioManager : MonoBehaviour
{
    public static float MusicVolume { get; private set; }
    public static float EffectsVolume { get; private set; }
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup effectsMixerGroup;
    [SerializeField] private TextMeshProUGUI musicSliderText;
    [SerializeField] private TextMeshProUGUI effectsSliderText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private Toggle muteToggleMusic;
    [SerializeField] private Toggle muteToggleEffects;


    private void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
            return;

        LoadVolume();
    }

    public void OnMusicSliderValueChange()
    {
        MusicVolume = musicSlider.value;
        musicAudio.volume = musicSlider.value;
        musicSliderText.text = ((int)(musicSlider.value * 100)).ToString();
    }

    public void OnEffectSliderValueChange()
    {
        EffectsVolume = effectsSlider.value;
        AudioManager.Instance.SetAudioSourceVolume(effectsSlider.value);
        effectsSliderText.text = ((int)(effectsSlider.value * 100)).ToString();
    }

    public void UpdateAudioMixerValue()
    {
        musicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(MusicVolume) * 20);
        effectsMixerGroup.audioMixer.SetFloat("Effects Volume", Mathf.Log10(EffectsVolume) * 20);
    }

    public void SaveMusicAndEffectVolume()
    {
        Debug.Log(MusicVolume); //ovde je greska posle restarta, ako se odma klikne save default je 0, nece da se desi ali stavicu check
        PlayerPrefs.SetFloat("musicVolume", MusicVolume);
        PlayerPrefs.SetFloat("effectsVolume", EffectsVolume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
        effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume", 1f);
    }

    public void MuteMusicToggle(bool mute)
    {
        if (mute)
        {
            musicAudio.mute = true;
        }
        else
        {
            musicAudio.mute = false;
        }

        UpdateAudioMixerValue();
        SaveMusicAndEffectVolume();
    }

    public void MuteEffectsToggle(bool muteEffects)
    {
        if (muteEffects)
        {
            AudioManager.Instance.MuteAudioSource();
        }
        else
        {
            AudioManager.Instance.UnmuteAudioSource();
        }

        UpdateAudioMixerValue();
        SaveMusicAndEffectVolume();
    }

}
