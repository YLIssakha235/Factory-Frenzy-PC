using UnityEngine;
using UnityEngine.EventSystems;

// small helper added on each button by UISounds: sound when the mouse goes over the button
public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundFX.Play("ui_hover");
    }
}
