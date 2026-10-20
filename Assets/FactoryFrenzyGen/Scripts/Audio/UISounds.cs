using UnityEngine;
using UnityEngine.UI;

// Factory Frenzy – click and hover sounds on EVERY button of a Canvas, and the music.
// Put it on the Canvas (RaceUI, LevelMenuUI...). No need to touch the buttons one by one.
public class UISounds : MonoBehaviour
{
    public bool playMusic = true;

    void Start()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(() => SoundFX.Play("ui_click"));
            if (button.GetComponent<HoverSound>() == null) button.gameObject.AddComponent<HoverSound>();
        }
        if (playMusic) SoundFX.PlayMusic("music_loop");
    }
}
