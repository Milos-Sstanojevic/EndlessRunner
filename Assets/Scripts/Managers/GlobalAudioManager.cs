using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GlobalAudioManager : MonoBehaviour
{
    private const string MusicVolumePref = "musicVolume";
    private const string EffectsVolumePref = "effectsVolume";
    private const string AudioMixerMusicVolume = "Music Volume";
    private const string AudioMixerEffectsVolume = "Effects Volume";
    private const int ZeroVolume = 0;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup effectsMixerGroup;
    [SerializeField] private TextMeshProUGUI musicSliderText;
    [SerializeField] private TextMeshProUGUI effectsSliderText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private Toggle muteToggleMusic;
    [SerializeField] private Toggle muteToggleEffects;
    public float MusicVolume { get; private set; }
    public float EffectsVolume { get; private set; }


    private void Start()
    {
        if (!PlayerPrefs.HasKey(MusicVolumePref))
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
        musicMixerGroup.audioMixer.SetFloat(AudioMixerMusicVolume, Mathf.Log10(MusicVolume) * 20);
        effectsMixerGroup.audioMixer.SetFloat(AudioMixerEffectsVolume, Mathf.Log10(EffectsVolume) * 20);
    }

    public void SaveMusicAndEffectVolume()
    {
        PlayerPrefs.SetFloat(MusicVolumePref, MusicVolume);
        PlayerPrefs.SetFloat(EffectsVolumePref, EffectsVolume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat(MusicVolumePref);
        effectsSlider.value = PlayerPrefs.GetFloat(EffectsVolumePref);
    }

    public void MuteMusicToggle(bool mute)
    {
        if (mute)
        {
            musicAudio.mute = true;
            MusicVolume = ZeroVolume;
        }
        else
        {
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumePref);
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
            EffectsVolume = ZeroVolume;
        }
        else
        {
            AudioManager.Instance.UnmuteAudioSource();
            EffectsVolume = PlayerPrefs.GetFloat(EffectsVolumePref);
        }

        UpdateAudioMixerValue();
        SaveMusicAndEffectVolume();
    }

}
