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
    
    void attemptMove(Move move)
    {
        //if promotion
        if ((move.piece == Piece.WhitePawn || move.piece == Piece.BlackPawn) && (move.destRow == 0 || move.destRow == 7))
        {
            //if promoting to queen is legal, open promote window
            move.pawnPromotionTarget = 'Q';
            if (bc.isLegalMove(move))
            {
                //open promotion window
                ui.openPromotionWindow(move);
                //snap pawn to dest square
                ui.pieces[(move.startRow, move.startCol)].GetComponent<RectTransform>().anchoredPosition = ui_controller.GridToPosition(new Vector2Int(move.destRow, move.destCol));
            }
            else
            {
                if (homeRow == move.startRow && homeCol == move.startCol) //this piece is attempting to move
                {
                    //snap back to home
                    trans.anchoredPosition = ui_controller.GridToPosition(new Vector2Int(homeRow, homeCol));
                }
                else //another piece is attempting to move
                {
                    ui.selectSquare(homeRow, homeCol);
                }
            }
        }
        else{ //send the move normally
            bool isLegal = bc.SendMove(move);
            if (isLegal)
            {
                //make the move
                ui.makeMoveOnUI(move);
            }
            else
            {
                if (homeRow == move.startRow && homeCol == move.startCol) //this piece is attempting to move
                {
                    //snap back to home
                    trans.anchoredPosition = ui_controller.GridToPosition(new Vector2Int(homeRow, homeCol));
                }
                else //another piece is attempting to move
                {
                    ui.selectSquare(homeRow, homeCol);
                }
            }
        }
    }

    public void onDragEnd()
    {
        Vector2Int gridPos = ui_controller.PositionToGrid(trans.anchoredPosition);
        //construct the move
        Move move = new Move(homeRow, homeCol, gridPos[0], gridPos[1], piece, "");
        
        attemptMove(move);
    }

    public void onClickDown(Vector2 mousePos)
    {
        if(!ui.promotionWindowIsOpen){
            if(ui.selectedSquare == (-1, -1))
            {
                ui.selectSquare(homeRow, homeCol);
            }
            else //there is a selected square
            {
                //if capturing me is a legal move, then make the move
                (int srow, int scol) = ui.selectedSquare;
                Move move = new Move(srow, scol, homeRow, homeCol, ui.selectedPiece, "");
                
                attemptMove(move);
            }
        }
    }
}
