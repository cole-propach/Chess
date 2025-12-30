using UnityEngine;
using UnityEngine.EventSystems;

public class promotion_button : MonoBehaviour, IPointerClickHandler
{
    ui_controller ui;
    board_controller bc;
    public char promotionTarget;

    void Start()
    {
        ui = GameObject.Find("ui controller").GetComponent<ui_controller>();
        bc = GameObject.Find("board controller").GetComponent<board_controller>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //construct move
        Move promotion = ui.potentialPromotionMove;
        promotion.pawnPromotionTarget = promotionTarget;
        //send move to bc
        bc.SendMove(promotion);
        //send move to ui
        ui.makeMoveOnUI(promotion);
        //close window
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
