using System.Collections.Generic;
using System.Data;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ui_controller : MonoBehaviour
{
    public Dictionary<(int, int), GameObject> pieces = new Dictionary<(int, int), GameObject>();
    public GameObject[,] dots = new GameObject[8,8];
    GameObject P;
    GameObject Dot;
    RectTransform boardRect;
    board_controller bc;
    RectTransform canvasRect;
    Canvas canvas;
    public (int, int) selectedSquare = (-1, -1);
    public Piece selectedPiece;
    void Start()
    {
        GameObject c = GameObject.Find("Canvas");
        canvas = c.GetComponent<Canvas>();
        bc = GameObject.Find("board controller").GetComponent<board_controller>();
        //position the board
        GameObject board = GameObject.Find("Board");
        boardRect = board.GetComponent<RectTransform>();
        canvasRect = boardRect.parent.GetComponent<RectTransform>();

        boardRect.anchorMin = new Vector2(0.5f, 0.5f);
        boardRect.anchorMax = new Vector2(0.5f, 0.5f);
        boardRect.pivot = new Vector2(0.5f, 0.5f);
        boardRect.anchoredPosition = Vector2.zero;

        float boardHeight = canvasRect.rect.height;
        boardRect.sizeDelta = new Vector2(boardHeight, boardHeight);

        //create every dot and hide it
        Dot = Resources.Load<GameObject>("Prefabs/dot");
        placeDots();

        //populate the board with piece game objects
        P = Resources.Load<GameObject>("Prefabs/Piece");
        populateBoard();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint
            );

            Vector2Int gridClickPos = PositionToGrid(localPoint);

            //if there are no pieces there
            if(!pieces.ContainsKey((gridClickPos[0], gridClickPos[1]))){
                //if there is a selected square, attempt a move
                if (selectedSquare != (-1, -1))
                {
                    (int srow, int scol) = selectedSquare;
                    Move move = new Move(srow, scol, gridClickPos[0], gridClickPos[1], pieces[(srow, scol)].GetComponent<piece_controller>().piece, "");
                    if (bc.SendMove(move)) //if move is legal
                    {
                        makeMoveOnUI(move);
                    }
                }
                unselectSquares();
            }
        }
    }

    void teleportPieceObject(int startRow, int startCol, int destRow, int destCol)
    {
        if (startRow == destRow && startCol == destCol)
        {
            return;
        }
        GameObject movingPiece = pieces[(startRow, startCol)];
        if (pieces.ContainsKey((destRow, destCol))) //there is a piece at the dest, capture it
        {
            GameObject capturedPiece = pieces[(destRow, destCol)];
            pieces.Remove((destRow, destCol));
            Destroy(capturedPiece);
        }
        //move the piece
        pieces.Remove((startRow, startCol));
        pieces[(destRow, destCol)] = movingPiece;
        movingPiece.GetComponent<piece_controller>().homeRow = destRow;
        movingPiece.GetComponent<piece_controller>().homeCol = destCol;
        movingPiece.GetComponent<RectTransform>().anchoredPosition = GridToPosition(new Vector2Int(destRow, destCol));
    }

    public void makeMoveOnUI(Move move)
    {
        GameObject movingPiece = pieces[(move.startRow, move.startCol)];
        Piece piece = movingPiece.GetComponent<piece_controller>().piece;

        //detect captures
        //if destination square has a piece in it, delete that piece object
        if (isPieceAt(move.destRow, move.destCol))
        {
            GameObject capturedPiece = pieces[(move.destRow, move.destCol)];
            pieces.Remove((move.destRow, move.destCol));
            Destroy(capturedPiece);
        }
        else{//no piece at destination
            //if this is enpassant, delete the correct pawn game object
            if(move.destRow != move.destCol){
                if (piece == Piece.WhitePawn)
                {
                    if (isPieceAt(move.destRow - 1, move.destCol))
                    {
                        GameObject potentialPawn = pieces[(move.destRow - 1, move.destCol)];
                        if(potentialPawn.GetComponent<piece_controller>().piece == Piece.BlackPawn){
                            pieces.Remove((move.destRow - 1, move.destCol));
                            Destroy(potentialPawn);
                        }
                    }
                }
                else if (piece == Piece.BlackPawn)
                {
                    if (isPieceAt(move.destRow + 1, move.destCol))
                    {
                        GameObject potentialPawn = pieces[(move.destRow + 1, move.destCol)];
                        if(potentialPawn.GetComponent<piece_controller>().piece == Piece.WhitePawn){
                            pieces.Remove((move.destRow + 1, move.destCol));
                            Destroy(potentialPawn);
                        }
                    }
                }
            }
            //if this is castling, move the correct rook as well
            if (math.abs(move.startCol-move.destCol) == 2) //moving 2 squares horizontally
            {
                if (move.piece == Piece.WhiteKing)
                {
                    //kingside
                    if (move.startCol < move.destCol) //moving to the right
                    {
                        teleportPieceObject(0, 7, 0, 5);
                    }
                    //queenside
                    else
                    {
                        teleportPieceObject(0, 0, 0, 3);
                    }
                }
                else if (move.piece == Piece.BlackKing)
                {
                    //kingside
                    if (move.startCol < move.destCol) //moving to the right
                    {
                        teleportPieceObject(7, 7, 7, 5);
                    }
                    //queenside
                    else
                    {
                        teleportPieceObject(7, 0, 7, 3);
                    }
                }
            }
        }
        unselectSquares();
        //move to dest square
        pieces.Remove((move.startRow, move.startCol));
        movingPiece.GetComponent<piece_controller>().homeRow = move.destRow;
        movingPiece.GetComponent<piece_controller>().homeCol = move.destCol;
        movingPiece.GetComponent<RectTransform>().anchoredPosition = GridToPosition(new Vector2Int(move.destRow, move.destCol));
        pieces[(move.destRow, move.destCol)] = movingPiece;
    }

    public bool isPieceAt(int row, int col)
    {
        return pieces.ContainsKey((row, col));
    }

    void showMyDots(List<Move> moves)
    {
        foreach(Move move in moves)
        {
            GameObject d = dots[move.destRow, move.destCol];
            d.GetComponent<dot_controller>().Show();
        }
    }

    void hideMyDots(List<Move> moves)
    {
        foreach(Move move in moves)
        {
            dots[move.destRow, move.destCol].GetComponent<dot_controller>().Hide();
        }
    }

    public void unselectSquares()
    {
        //unhighlight selected square
        selectedSquare = (-1, -1);
        foreach (GameObject dot in dots)
        {
            dot.GetComponent<dot_controller>().Hide();
        }
    }

    public void selectSquare(int row, int col)
    {
        unselectSquares();
        selectedSquare = (row, col);
        selectedPiece = pieces[(row, col)].GetComponent<piece_controller>().piece;
        //highlight selected square

        //show dots for all this squares legal moves
        showMyDots(bc.getMovesForSquare(row, col));
    }

    void placeDots()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                GameObject d = Instantiate(Dot, Vector3.zero, Quaternion.identity, boardRect);
                d.GetComponent<RectTransform>().anchoredPosition = GridToPosition(new Vector2Int(row, col));
                d.GetComponent<dot_controller>().Hide();
                dots[row, col] = d;
            }
        }
    }

    GameObject newPiece(Piece piece, int row, int col)
    {
        Color color = board_controller.getPieceColor(piece);
        GameObject p = Instantiate(P, Vector3.zero, Quaternion.identity, boardRect);
        p.GetComponent<RectTransform>().anchoredPosition = GridToPosition(new Vector2Int(row, col));
        piece_controller controller = p.GetComponent<piece_controller>();
        controller.homeRow = row;
        controller.homeCol = col;
        controller.piece = piece;
        
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
