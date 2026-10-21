using UnityEngine;
using UnityEngine.UI;

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
