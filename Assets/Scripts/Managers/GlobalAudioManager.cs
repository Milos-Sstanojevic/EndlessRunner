using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioManager : MonoBehaviour
{
    public static float MusicVolume { get; private set; }
    public static float EffectsVolume { get; private set; }
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup effectsMixerGroup;
    [SerializeField] private TextMeshProUGUI musicSliderText;
    [SerializeField] private TextMeshProUGUI effectsSliderText;
    [SerializeField] private AudioSource musicAudio;
    //ovde treba nekako da pozovem moj audio manager i dodelim mu vrednost za volume

    public void OnMusicSliderValueChange(float value)
    {
        MusicVolume = value;
        musicAudio.volume = value;//testiraj
        musicSliderText.text = ((int)(value * 100)).ToString();
    }

    public void OnEffectSliderValueChange(float value)
    {
        EffectsVolume = value;
        //ovde ce verovatno za audio manager jer je on za sve effekte
        effectsSliderText.text = ((int)(value * 100)).ToString();
    }

    public void UpdateAudioMixerValue()
    {
        musicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(MusicVolume) * 20);
        effectsMixerGroup.audioMixer.SetFloat("Effects Volume", Mathf.Log10(EffectsVolume) * 20);
    }

}
