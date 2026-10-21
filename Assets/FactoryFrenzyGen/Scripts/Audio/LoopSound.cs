using UnityEngine;

// Factory Frenzy – a sound that plays forever around an object, louder when you are close.
// Put it on the Fan prefab (sound "fan_loop") or the Moving platform (sound "platform_hum_loop").
public class LoopSound : MonoBehaviour
{
    public string sound = "fan_loop";
    public float volume = 0.6f;
    public float hearDistance = 15f;   // you hear nothing further than this

    AudioSource source;

    void Start()
    {
        SoundFX.LoadSettings();
        AudioClip clip = SoundFX.Get(sound);
        if (clip == null) return;

        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.spatialBlend = 1f;                          // 3D sound
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 2f;
        source.maxDistance = hearDistance;
        source.time = Random.Range(0f, clip.length);       // two fans do not sound exactly the same
        source.Play();
    }

    void Update()
    {
        // follows the Effects setting (on/off and volume) even if it changes during the game
        if (source != null) source.volume = volume * SoundFX.EffectsVolume;
    }
}
