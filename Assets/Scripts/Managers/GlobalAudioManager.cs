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
        LoadMuteState();
    }

    public void OnMusicSliderValueChange()
    {
        MusicVolume = musicSlider.value;
        UpdateMusicAudioMixerValue(MusicVolume);
        musicSliderText.text = ((int)(musicSlider.value * 100)).ToString();
    }

    public void OnEffectSliderValueChange()
    {
        EffectsVolume = effectsSlider.value;
        UpdateEffectsAudioMixerValue(EffectsVolume);
        effectsSliderText.text = ((int)(effectsSlider.value * 100)).ToString();
    }

    public void SaveMusicAndEffectVolume()
    {
        PlayerPrefs.SetFloat(MusicVolumePref, musicSlider.value);
        PlayerPrefs.SetFloat(EffectsVolumePref, effectsSlider.value);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat(MusicVolumePref);
        effectsSlider.value = PlayerPrefs.GetFloat(EffectsVolumePref);
    }

    private void LoadMuteState()
    {
        muteToggleMusic.isOn = PlayerPrefs.GetInt("MusicMute", 0) == 1;
        muteToggleEffects.isOn = PlayerPrefs.GetInt("EffectsMute", 0) == 1;
        MuteMusicToggle(muteToggleMusic.isOn);
        MuteEffectsToggle(muteToggleEffects.isOn);
    }

    public void MuteMusicToggle(bool mute)
    {
        musicAudio.mute = mute;
        PlayerPrefs.SetInt("MusicMute", mute ? 1 : 0);
    }

    public void MuteEffectsToggle(bool mute)
    {
        if (mute)
            AudioManager.Instance.MuteAudioSource();
        else
            AudioManager.Instance.UnmuteAudioSource();

        PlayerPrefs.SetInt("EffectsMute", mute ? 1 : 0);
    }

    private void UpdateMusicAudioMixerValue(float volume)
    {
        musicMixerGroup.audioMixer.SetFloat(AudioMixerMusicVolume, Mathf.Log10(volume) * 20);
    }

    private void UpdateEffectsAudioMixerValue(float volume)
    {
        effectsMixerGroup.audioMixer.SetFloat(AudioMixerEffectsVolume, Mathf.Log10(volume) * 20);
    }

}