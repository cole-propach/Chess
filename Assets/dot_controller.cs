using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class dot_controller : MonoBehaviour
{
    Image image;
    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Show()
    {
        image.enabled = true;
    }

    public void Hide()
    {
        image.enabled = false;
    }
}
