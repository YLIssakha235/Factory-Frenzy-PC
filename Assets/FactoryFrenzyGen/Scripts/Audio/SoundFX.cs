using System.Collections.Generic;
using UnityEngine;


public class SoundFX : MonoBehaviour
{
    public static bool allOn = true;
    public static float masterVolume = 1f;
    public static bool musicOn = true;
    public static float musicVolume = 0.35f;
    public static bool effectsOn = true;
    public static float effectsVolume = 1f;

    static SoundFX instance;
    static bool settingsLoaded = false;
    static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    AudioSource effects;
    AudioSource music;

    public static float EffectsVolume
    {
        get { return effectsOn ? effectsVolume : 0f; }
    }

    static SoundFX Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SoundFX");
                DontDestroyOnLoad(go);              
                instance = go.AddComponent<SoundFX>();
                instance.effects = go.AddComponent<AudioSource>();
                instance.music = go.AddComponent<AudioSource>();
                instance.music.loop = true;
                LoadSettings();
            }
            return instance;
        }
    }


    public static void LoadSettings()
    {
        if (settingsLoaded) return;
        settingsLoaded = true;
        allOn = PlayerPrefs.GetInt("SoundAllOn", 1) == 1;
        masterVolume = PlayerPrefs.GetFloat("SoundMasterVolume", 1f);
        musicOn = PlayerPrefs.GetInt("SoundMusicOn", 1) == 1;
        musicVolume = PlayerPrefs.GetFloat("SoundMusicVolume", 0.35f);
        effectsOn = PlayerPrefs.GetInt("SoundEffectsOn", 1) == 1;
        effectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
        Apply();
    }

    public static void SaveAndApply()
    {
        PlayerPrefs.SetInt("SoundAllOn", allOn ? 1 : 0);
        PlayerPrefs.SetFloat("SoundMasterVolume", masterVolume);
        PlayerPrefs.SetInt("SoundMusicOn", musicOn ? 1 : 0);
        PlayerPrefs.SetFloat("SoundMusicVolume", musicVolume);
        PlayerPrefs.SetInt("SoundEffectsOn", effectsOn ? 1 : 0);
        PlayerPrefs.SetFloat("SoundEffectsVolume", effectsVolume);
        PlayerPrefs.Save();
        Apply();
    }

    static void Apply()
    {
        AudioListener.volume = allOn ? masterVolume : 0f;
        if (instance != null) instance.music.volume = musicOn ? musicVolume : 0f;
    }


    public static AudioClip Get(string name)
    {
        if (!clips.ContainsKey(name))
        {
            AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);
            if (clip == null) Debug.LogWarning("SoundFX: no sound called " + name + " in Assets/Resources/Sounds");
            clips[name] = clip;
        }
        return clips[name];
    }

    public static void Play(string name, float volume = 1f)
    {
        AudioClip clip = Get(name);
        if (clip != null && EffectsVolume > 0f) Instance.effects.PlayOneShot(clip, volume * EffectsVolume);
    }


    public static void Play(string name, Vector3 position, float volume = 1f)
    {
        LoadSettings();
        AudioClip clip = Get(name);
        if (clip == null || EffectsVolume <= 0f) return;

        GameObject go = new GameObject("Sound " + name);
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * EffectsVolume;
        source.spatialBlend = 1f;                    
        source.minDistance = 8f;                   
        source.maxDistance = 40f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.Play();
        Destroy(go, clip.length + 0.1f);             
    }

    public static void PlayMusic(string name)
    {
        AudioClip clip = Get(name);
        AudioSource source = Instance.music;
        if (clip == null || (source.clip == clip && source.isPlaying)) return; 
        source.clip = clip;
        source.volume = musicOn ? musicVolume : 0f;
        source.Play();
    }

    public static void StopMusic()
    {
        if (instance != null) instance.music.Stop();
    }
}
