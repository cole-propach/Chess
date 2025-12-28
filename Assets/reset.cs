using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class reset : MonoBehaviour, IPointerDownHandler
{
    board_controller bc;
    ui_controller ui;
    void Start()
    {
        bc = GameObject.Find("board controller").GetComponent<board_controller>();
        ui = GameObject.Find("ui controller").GetComponent<ui_controller>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        resetGame();
    }

    void resetGame()
    {
        ui.resetGameUI();
        bc.resetGame();
    }
}
