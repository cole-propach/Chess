using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class change_sprite_on_hover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    Image image;
    Sprite unhovered;
    Sprite hovered;

    void Awake()
    {
        image = GetComponent<Image>();
        unhovered = Resources.Load<Sprite>("resetButton");
        hovered = Resources.Load<Sprite>("resetButtonSelected");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hovered;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = unhovered;
    }
}