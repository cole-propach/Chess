using UnityEngine;

public class piece_controller : MonoBehaviour
{
    RectTransform trans;
    ui_controller ui;

    public int homeRow = -1;
    public int homeCol = -1;
    void Start()
    {
        trans = GetComponent<RectTransform>();
    }

    public void onDragEnd()
    {
        trans.anchoredPosition = ui_controller.GridToPosition(ui_controller.PositionToGrid(trans.anchoredPosition));
    }
}
