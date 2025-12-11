using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class ui_controller : MonoBehaviour
{
    public Dictionary<(int, int), GameObject> pieces = new Dictionary<(int, int), GameObject>();
    GameObject Piece;
    RectTransform boardRect;
    void Start()
    {
        //position the board
        GameObject board = GameObject.Find("Board");
        boardRect = board.GetComponent<RectTransform>();
        RectTransform canvasRect = boardRect.parent.GetComponent<RectTransform>();

        boardRect.anchorMin = new Vector2(0.5f, 0.5f);
        boardRect.anchorMax = new Vector2(0.5f, 0.5f);
        boardRect.pivot = new Vector2(0.5f, 0.5f);
        boardRect.anchoredPosition = Vector2.zero;

        float boardHeight = canvasRect.rect.height;
        boardRect.sizeDelta = new Vector2(boardHeight, boardHeight);

        //populate the board with piece game objects
        Piece = Resources.Load<GameObject>("Prefabs/Piece");
        populateBoard();
    }

    GameObject newPiece(Piece piece, int row, int col)
    {
        Color color = board_controller.getPieceColor(piece);
        GameObject p = Instantiate(Piece, Vector3.zero, Quaternion.identity, boardRect);
        p.GetComponent<RectTransform>().anchoredPosition = GridToPosition(new Vector2Int(row, col));
        piece_controller controller = p.GetComponent<piece_controller>();
        controller.homeRow = row;
        controller.homeCol = col;
        
        string colorString = (color == Color.White) ? "white" : "black";
        string pieceString = "";
        string pieceandcolorString = board_controller.pieceToString(piece);
        switch (pieceandcolorString[1])
        {
            case 'P':
                pieceString = "Pawn";
            break;
            case 'N':
                pieceString = "Knight";
            break;
            case 'B':
                pieceString = "Bishop";
            break;
            case 'R':
                pieceString = "Rook";
            break;
            case 'Q':
                pieceString = "Queen";
            break;
            case 'K':
                pieceString = "King";
            break;
        }
        string spriteName = colorString + pieceString;
        Sprite sprite = Resources.Load<Sprite>(spriteName);
        p.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        return p;
    }

    void populateBoard()
    {
        //place pawns
        bool done = false;
        int row = 1; //loop through 2 rows, 1 per color
        while (!done)
        {
            for (int i = 0; i < 8; i++)
            {
                pieces[(row, i)] = (row == 1)? newPiece(global::Piece.WhitePawn, row, i) : newPiece(global::Piece.BlackPawn, row, i);
            }
            if(row == 6) done = true;
            row = 6;
        }
        //place other pieces
        done = false;
        row = 0;
        while (!done)
        {
            pieces[(row, 0)] = (row == 0)?  newPiece(global::Piece.WhiteRook, row, 0) : newPiece(global::Piece.BlackRook, row, 0);
            pieces[(row, 7)] = (row == 0)?  newPiece(global::Piece.WhiteRook, row, 7) : newPiece(global::Piece.BlackRook, row, 7);

            pieces[(row, 1)] = (row == 0)?  newPiece(global::Piece.WhiteKnight, row, 1) : newPiece(global::Piece.BlackKnight, row, 1);
            pieces[(row, 6)] = (row == 0)?  newPiece(global::Piece.WhiteKnight, row, 6) : newPiece(global::Piece.BlackKnight, row, 6);

            pieces[(row, 2)] = (row == 0)?  newPiece(global::Piece.WhiteBishop, row, 2) : newPiece(global::Piece.BlackBishop, row, 2);
            pieces[(row, 5)] = (row == 0)?  newPiece(global::Piece.WhiteBishop, row, 5) : newPiece(global::Piece.BlackBishop, row, 5);

            pieces[(row, 3)] = (row == 0)?  newPiece(global::Piece.WhiteQueen, row, 3) : newPiece(global::Piece.BlackQueen, row, 3);
            pieces[(row, 4)] = (row == 0)?  newPiece(global::Piece.WhiteKing, row, 4) : newPiece(global::Piece.BlackKing, row, 4);

            if(row == 7) done = true;
            row = 7;
        }
    }

    public static Vector2Int PositionToGrid(Vector2 pos)
    {
        const int squareSize = 32;
        const int boardSize = 8;
        const int halfBoard = (boardSize * squareSize) / 2;

        //shift origin from center to bottom-left
        float shiftedX = pos.x + halfBoard;
        float shiftedY = pos.y + halfBoard;

        int col = Mathf.FloorToInt(shiftedX / squareSize);
        int row = Mathf.FloorToInt(shiftedY / squareSize);

        //clamp to board
        col = Mathf.Clamp(col, 0, boardSize - 1);
        row = Mathf.Clamp(row, 0, boardSize - 1);

        return new Vector2Int(row, col);
    }

    public static Vector2 GridToPosition(Vector2Int rowcol)
    {
        int row = rowcol.x;
        int col = rowcol.y;
        const int squareSize = 32;
        const int boardSize = 8;
        const int halfBoard = (boardSize * squareSize) / 2; // 128

        // Position of the bottom-left *corner* of this square
        float x = col * squareSize;
        float y = row * squareSize;

        // Move to square center
        x += squareSize * 0.5f;
        y += squareSize * 0.5f;

        // Shift from bottom-left origin back to center origin
        x -= halfBoard;
        y -= halfBoard;

        return new Vector2(x, y);
    }
}
