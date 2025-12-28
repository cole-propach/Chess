using Unity.VisualScripting;
using UnityEngine;

public class piece_controller : MonoBehaviour
{
    RectTransform trans;
    ui_controller ui;
    board_controller bc;

    public int homeRow = -1;
    public int homeCol = -1;
    public Piece piece;
    void Start()
    {
        trans = GetComponent<RectTransform>();
        bc = GameObject.Find("board controller").GetComponent<board_controller>();
        ui = GameObject.Find("ui controller").GetComponent<ui_controller>();
    }

    public void onDragEnd()
    {
        Vector2Int gridPos = ui_controller.PositionToGrid(trans.anchoredPosition);
        //construct the move
        Move move = new Move(homeRow, homeCol, gridPos[0], gridPos[1], piece, "");
        bool isLegal = bc.SendMove(move);
        if (isLegal)
        {
            //make the move
            ui.makeMoveOnUI(move);
        }
        else
        {
            //move to home square
            trans.anchoredPosition = ui_controller.GridToPosition(new Vector2Int(homeRow, homeCol));
        }
    }

    public void onClickDown(Vector2 mousePos)
    {
        if(ui.selectedSquare == (-1, -1))
        {
            ui.selectSquare(homeRow, homeCol);
        }
        else //there is a selected square
        {
            //if capturing me is a legal move, then make the move
            (int srow, int scol) = ui.selectedSquare;
            Move move = new Move(srow, scol, homeRow, homeCol, ui.selectedPiece, "");

            if (bc.SendMove(move)) //if capturing is legal
            {
                ui.makeMoveOnUI(move);
            }
            //otherwise, just select me
            else
            {
                ui.selectSquare(homeRow, homeCol);
            }
        }
    }
}
