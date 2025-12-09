using UnityEngine;

public class ui_controller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //position the board
        RectTransform boardRect = GameObject.Find("Board").GetComponent<RectTransform>();
        RectTransform canvasRect = boardRect.parent.GetComponent<RectTransform>();

        // Center the board (same as before)
        boardRect.anchorMin = new Vector2(0.5f, 0.5f);
        boardRect.anchorMax = new Vector2(0.5f, 0.5f);
        boardRect.pivot = new Vector2(0.5f, 0.5f);
        boardRect.anchoredPosition = Vector2.zero;

        float boardHeight = canvasRect.rect.height;
        boardRect.sizeDelta = new Vector2(boardHeight, boardHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
