using UnityEngine;

public class LoopSound : MonoBehaviour
{
    public string sound = "fan_loop";
    public float volume = 0.6f;
    public float hearDistance = 15f;   

    AudioSource source;

    void Start()
    {
        SoundFX.LoadSettings();
        AudioClip clip = SoundFX.Get(sound);
        if (clip == null) return;

        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.spatialBlend = 1f;                         
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 2f;
        source.maxDistance = hearDistance;
        source.time = Random.Range(0f, clip.length);       
        source.Play();
    }

    void Update()
    {
        if (source != null) source.volume = volume * SoundFX.EffectsVolume;
    }
}
