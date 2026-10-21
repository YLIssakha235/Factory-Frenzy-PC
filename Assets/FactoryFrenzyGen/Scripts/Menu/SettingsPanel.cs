using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Factory Frenzy – the Settings panel: 3 rows (All sounds / Music / Effects), each with an ON/OFF toggle and a volume slider.
// Made by Factory Frenzy > Build Pause + Settings. The values are saved by SoundFX (PlayerPrefs).
public class SettingsPanel : MonoBehaviour
{
    public Toggle allToggle, musicToggle, effectsToggle;
    public Slider allSlider, musicSlider, effectsSlider;
    public TextMeshProUGUI allValue, musicValue, effectsValue;   // "80%"

    [HideInInspector] public GameObject returnTo;               // panel to show again when we close (the pause menu)
    bool ready = false;

    void Awake()
    {
        SoundFX.LoadSettings();

        // show the saved values
        allToggle.isOn = SoundFX.allOn;
        allSlider.value = SoundFX.masterVolume;
        musicToggle.isOn = SoundFX.musicOn;
        musicSlider.value = SoundFX.musicVolume;
        effectsToggle.isOn = SoundFX.effectsOn;
        effectsSlider.value = SoundFX.effectsVolume;
        UpdateTexts();

        // when the player changes something
        allToggle.onValueChanged.AddListener(on => { SoundFX.allOn = on; Changed(); });
        allSlider.onValueChanged.AddListener(v => { SoundFX.masterVolume = v; Changed(); });
        musicToggle.onValueChanged.AddListener(on => { SoundFX.musicOn = on; Changed(); });
        musicSlider.onValueChanged.AddListener(v => { SoundFX.musicVolume = v; Changed(); });
        effectsToggle.onValueChanged.AddListener(on => { SoundFX.effectsOn = on; Changed(); });
        effectsSlider.onValueChanged.AddListener(v => { SoundFX.effectsVolume = v; Changed(); SoundFX.Play("ui_hover"); }); // hear the new level
        ready = true;
    }

    void Changed()
    {
        if (!ready) return;
        SoundFX.SaveAndApply();
        UpdateTexts();
    }

    void UpdateTexts()
    {
        allValue.text = Percent(SoundFX.allOn, SoundFX.masterVolume);
        musicValue.text = Percent(SoundFX.musicOn, SoundFX.musicVolume);
        effectsValue.text = Percent(SoundFX.effectsOn, SoundFX.effectsVolume);
    }

    string Percent(bool on, float volume)
    {
        if (!on) return "OFF";
        return Mathf.RoundToInt(volume * 100) + "%";
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    // BACK button
    public void Close()
    {
        gameObject.SetActive(false);
        if (returnTo != null) returnTo.SetActive(true);
        returnTo = null;
    }
}
