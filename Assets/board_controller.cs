using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

public enum Piece {
    Empty,
    WhitePawn, WhiteKnight, WhiteBishop, WhiteRook, WhiteQueen, WhiteKing,
    BlackPawn, BlackKnight, BlackBishop, BlackRook, BlackQueen, BlackKing
}

public enum Color
{
    White, Black, Empty
}

public struct Move{
    public int startRow;
    public int startCol;
    public int destRow;
    public int destCol;
    public Piece piece;
    public string name;
    public char pawnPromotionTarget;

    public Move(int startRow, int startCol, int destRow, int destCol, Piece piece, string name)
    {
        this.startRow = startRow;
        this.startCol = startCol;
        this.destRow = destRow;
        this.destCol = destCol;
        this.piece = piece;
        this.name = name;
        this.pawnPromotionTarget = '0';
    }

    public void printMove()
    {
        Debug.Log($"{name}: {piece} moves from ({startRow}, {startCol}) to ({destRow}, {destCol})");
    }
}

public class board_controller : MonoBehaviour
{
    ui_controller ui;
    //  (0, 0) is bottom left, or a1
    //  (7, 7) is top right, or h8
    Piece[,] board = new Piece[8, 8];

    (int, int) enPassantDest = (-1, -1);

    public static class CastlingRights
    {
        public static bool white_kingside = true;
        public static bool white_queenside = true;
        public static bool black_kingside = true;
        public static bool black_queenside = true;
    };

    Dictionary<(int, int), List<Move>> currentLegalMoves = new Dictionary<(int, int), List<Move>>();
    Color colorToMove = Color.White;

    void Start()
    {
        ui = GameObject.Find("ui controller").GetComponent<ui_controller>();
        //put every piece on the board
        resetGame();
    }

    public void resetGame()
    {
        CastlingRights.white_kingside = true;
        CastlingRights.white_queenside = true;
        CastlingRights.black_kingside = true;
        CastlingRights.black_queenside = true;

        enPassantDest = (-1, -1);

        colorToMove = Color.White;

        clearBoard();
        populateBoard();
        currentLegalMoves = generateMoves(colorToMove);
    }

    void clearBoard()
    {
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                board[r, c] = Piece.Empty;
            }
        }
    }

    public List<Move> getMovesForSquare(int row, int col)
    {
        if (!currentLegalMoves.ContainsKey((row, col)))
        {
            return new List<Move>();
        }
        else
            return currentLegalMoves[(row, col)];
    }

    void printMoves(Dictionary<(int, int), List<Move>> movesMap)
    {
        foreach (var entry in movesMap)
        {
            foreach (Move move in entry.Value)
            {
                move.printMove();
            }
        }
    }

    bool isInBounds(int row, int col)
    {
        return row >= 0 && col >= 0 && row <= 7 && col <= 7;
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
                board[row, i] = (row == 1)?  Piece.WhitePawn:  Piece.BlackPawn;
            }
            if(row == 6) done = true;
            row = 6;
        }
        //place other pieces
        done = false;
        row = 0;
        while (!done)
        {
            board[row, 0] = (row == 0)?  Piece.WhiteRook:  Piece.BlackRook;
            board[row, 7] = (row == 0)?  Piece.WhiteRook:  Piece.BlackRook;

            board[row, 1] = (row == 0)?  Piece.WhiteKnight:  Piece.BlackKnight;
            board[row, 6] = (row == 0)?  Piece.WhiteKnight:  Piece.BlackKnight;

            board[row, 2] = (row == 0)?  Piece.WhiteBishop:  Piece.BlackBishop;
            board[row, 5] = (row == 0)?  Piece.WhiteBishop:  Piece.BlackBishop;

            board[row, 3] = (row == 0)?  Piece.WhiteQueen:  Piece.BlackQueen;
            board[row, 4] = (row == 0)?  Piece.WhiteKing:  Piece.BlackKing;


            if(row == 7) done = true;
            row = 7;
        }
    }

    public static string pieceToString(Piece p)
    {
        switch (p)
        {
            case Piece.WhitePawn:   return "WP";
            case Piece.WhiteKnight: return "WN";
            case Piece.WhiteBishop: return "WB";
            case Piece.WhiteRook:   return "WR";
            case Piece.WhiteQueen:  return "WQ";
            case Piece.WhiteKing:   return "WK";

            case Piece.BlackPawn:   return "BP";
            case Piece.BlackKnight: return "BN";
            case Piece.BlackBishop: return "BB";
            case Piece.BlackRook:   return "BR";
            case Piece.BlackQueen:  return "BQ";
            case Piece.BlackKing:   return "BK";

            default: return ".";
        }
    }

    public static Piece stringToPiece(string s)
    {
        switch (s)
        {
            case "WP": return Piece.WhitePawn;
            case "WN": return Piece.WhiteKnight;
            case "WB": return Piece.WhiteBishop;
            case "WR": return Piece.WhiteRook;
            case "WQ": return Piece.WhiteQueen;
            case "WK": return Piece.WhiteKing;

            case "BP": return Piece.BlackPawn;
            case "BN": return Piece.BlackKnight;
            case "BB": return Piece.BlackBishop;
            case "BR": return Piece.BlackRook;
            case "BQ": return Piece.BlackQueen;
            case "BK": return Piece.BlackKing;

            default: return Piece.Empty;
        }
    }


    void printBoard()
    {
        Debug.Log("V this one is the real board V");
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        string boardString = "";

        for (int y = rows-1; y >= 0; y--)
        {
            for (int x = 0; x < cols; x++)
            {
                boardString += pieceToString(board[y, x]).PadRight(3);
            }
            boardString += "\n";
        }

        Debug.Log(boardString);
    }

    void printGivenBoard(Piece[,] b)
    {
        int rows = b.GetLength(0);
        int cols = b.GetLength(1);

        string boardString = "";

        for (int y = rows-1; y >= 0; y--)
        {
            for (int x = 0; x < cols; x++)
            {
                boardString += pieceToString(b[y, x]).PadRight(3);
            }
            boardString += "\n";
        }

        Debug.Log(boardString);
    }

    void teleportPiece(Piece[,] b, int startRow, int startCol, int destRow, int destCol)
    {
        if (startRow == destRow && startCol == destCol)
        {
            return;
        }
        b[destRow, destCol] = b[startRow, startCol];
        b[startRow, startCol] = Piece.Empty;
    }

    public bool isLegalMove(Move move)
    {
        bool hasKey = currentLegalMoves.ContainsKey((move.startRow, move.startCol));
        if(!hasKey) return false;
        List<Move> movesToCheck = currentLegalMoves[(move.startRow, move.startCol)];
        foreach (Move m in movesToCheck)
        {
            if (m.Equals(move))
            {
                return true;
            }
        }
        return false;
    }

    //ONLY make LEGAL moves
    //making illegal moves is undefined
    void makeMove(Move move)
    {
        //if the move ends in a rook start square, remove the relevant castling rights
        if (move.destRow == 0 && move.destCol == 0)
            CastlingRights.white_queenside = false;
        else if (move.destRow == 0 && move.destCol == 7)
            CastlingRights.white_kingside = false;
        else if (move.destRow == 7 && move.destCol == 0)
            CastlingRights.black_queenside = false;
        else if (move.destRow == 7 && move.destCol == 7)
            CastlingRights.black_kingside = false;

        bool isEnPassant = (move.destRow, move.destCol) == enPassantDest;
        enPassantDest = (-1, -1);
        Color color = getPieceColor(move.piece);
        string pieceString = pieceToString(move.piece);
        switch (pieceString[1])//piece char
        {
            case 'Q':
            case 'B':
            case 'N':
                //make move normally
                teleportPiece(board, move.startRow, move.startCol, move.destRow, move.destCol);
            break;
            case 'R':
                //revoke relevant castling rights
                if (color == Color.White && move.startRow == 0)
                {
                    if (move.startCol == 0)
                        CastlingRights.white_queenside = false;
                    else if (move.startCol == 7)
                        CastlingRights.white_kingside = false;
                }
                else if (color == Color.Black && move.startRow == 7)
                {
                    if (move.startCol == 0)
                        CastlingRights.black_queenside = false;
                    else if (move.startCol == 7)
                        CastlingRights.black_kingside = false;
                }
                teleportPiece(board, move.startRow, move.startCol, move.destRow, move.destCol);
            break;
            case 'P':
                //en passant
                if (isEnPassant)
                {
                    //remove the pawn behind the destination
                    board[move.startRow, move.destCol] = Piece.Empty;
                    teleportPiece(board, move.startRow, move.startCol, move.destRow, move.destCol);
                }
                //promotion
                else if (move.pawnPromotionTarget != '0')
                {
                    board[move.startRow, move.startCol] = Piece.Empty;
                    switch (move.pawnPromotionTarget)
                    {
                        case 'Q':
                            board[move.destRow, move.destCol] = (color == Color.White) ? Piece.WhiteQueen : Piece.BlackQueen;
                        break;
                        case 'R':
                            board[move.destRow, move.destCol] = (color == Color.White) ? Piece.WhiteRook : Piece.BlackRook;
                        break;
                        case 'B':
                            board[move.destRow, move.destCol] = (color == Color.White) ? Piece.WhiteBishop : Piece.BlackBishop;
                        break;
                        case 'N':
                            board[move.destRow, move.destCol] = (color == Color.White) ? Piece.WhiteKnight : Piece.BlackKnight;
                        break;
                    }
                }
                else
                {
                    //normal move
                    teleportPiece(board, move.startRow, move.startCol, move.destRow, move.destCol);
                }

                //if double push, update en passant destination
                if ( Mathf.Abs(move.startRow - move.destRow) == 2)
                {
                    int direction = (color == Color.White) ? -1 : 1;
                    enPassantDest = (move.destRow + direction, move.destCol);
                }
            break;
            case 'K':
                //revoke all castling rights
                if (color == Color.White)
                {
                    CastlingRights.white_kingside = false;
                    CastlingRights.white_queenside = false;
                }
                else
                {
                    CastlingRights.black_kingside = false;
                    CastlingRights.black_queenside = false;
                }
                //castling
                if ( Mathf.Abs(move.startCol - move.destCol) == 2)
                {
                    //kingside
                    if (move.startCol < move.destCol)
                    {
                        //king
                        teleportPiece(board, move.startRow, move.startCol, move.destRow, move.destCol);
                        //rook
                        teleportPiece(board, move.startRow, 7, move.startRow, 5);
                    }
                    //queenside
                    else
                    {
                        //king
                        teleportPiece(board, move.startRow, move.startCol, move.destRow, move.destCol);
                        //rook
                        teleportPiece(board, move.startRow, 0, move.startRow, 3);
                    }
                }
                //normal move
                else
                    teleportPiece(board, move.startRow, move.startCol, move.destRow, move.destCol);
            break;
        }
        //update color to move and re generate moves
        colorToMove = (colorToMove == Color.White) ? Color.Black : Color.White;
        currentLegalMoves = generateMoves(colorToMove);
        int numberOfLegalMoves = 0;
        foreach (KeyValuePair<(int, int), List<Move>> entry in currentLegalMoves)
        {
            numberOfLegalMoves += entry.Value.Count;
        }
        if (numberOfLegalMoves == 0) //there are no legal moves
        {
            endGame();
        }
    }

    void endGame()
    {
        // (white, black), true if that player won. if both are false it is stalemate
        (bool, bool) wins = (false, false);
        Color colorThatMightHaveWon = (colorToMove == Color.White) ? Color.Black : Color.White;
        bool thatColorDidWin = isInCheck(board, colorToMove);

        bool whiteWon = (colorThatMightHaveWon == Color.White) && thatColorDidWin;
        bool blackWon = (colorThatMightHaveWon == Color.Black) && thatColorDidWin;
        wins = (whiteWon, blackWon);
        
        ui.EndGameOnUI(wins);
    }

    public bool SendMove(Move move)
    {
        bool moveIsLegal = isLegalMove(move);
        //if move is illegal return false and do nothing
        if (!moveIsLegal)
        {
            return false;
        }
        //else move is legal
        else
        {
            //make move
            makeMove(move);
            //return true
            return true;
        }
    }


    public static Color getPieceColor(Piece p)
    {
        switch (p)
        {
            case Piece.WhitePawn:
            case Piece.WhiteKnight:
            case Piece.WhiteBishop:
            case Piece.WhiteRook:
            case Piece.WhiteQueen:
            case Piece.WhiteKing:
                return Color.White;

            case Piece.BlackPawn:
            case Piece.BlackKnight:
            case Piece.BlackBishop:
            case Piece.BlackRook:
            case Piece.BlackQueen:
            case Piece.BlackKing:
                return Color.Black;

            default:
                return Color.Empty;
        }
    }

    bool isInCheck(Piece[,] b, Color color)
    {
        Color opponentColor = (color == Color.White) ? Color.Black : Color.White;
        //scan in a bishop, rook, knight, pawn, and king move away from our king
        //if the specific piece is found, we must be in check
        bool kingFound = false;
        //find the square our king is on
        int kingRow = 0, kingCol = 0;
        for (kingRow = 0; kingRow < 8; kingRow++)
        {
            for (kingCol = 0; kingCol < 8; kingCol++)
            {
                if(b[kingRow, kingCol] == ((color == Color.White) ? Piece.WhiteKing : Piece.BlackKing))
                {
                    kingFound = true;
                    break;
                }
            }
            if (kingFound)
            {
                break;
            }
        }
        if (!kingFound)
        {
            Debug.Log("called isInCheck() on a board with no corresponding king!");
            printGivenBoard(b);
            return true;
        }
        //check a pawn capture away (color dependent) for enemy pawns
        int colorSign = (opponentColor == Color.White) ? -1 : 1;
        //if not out of bounds, check for pawns
        if (!((kingRow + colorSign) > 7 || (kingRow + colorSign) < 0))
        {
            //check left side if in bounds
            if (!((kingCol - 1) > 7 || (kingCol - 1) < 0))
            {
                if (b[kingRow + colorSign, kingCol - 1] == ((opponentColor == Color.White) ? Piece.WhitePawn : Piece.BlackPawn))
                {
                    return true;
                }
            }
            //check right side if in bounds
            if (!((kingCol + 1) > 7 || (kingCol + 1) < 0))
            {
                if (b[kingRow + colorSign, kingCol + 1] == ((opponentColor == Color.White) ? Piece.WhitePawn : Piece.BlackPawn))
                {
                    return true;
                }
            }
        }

        //check a king move away for an enemy king
        for (int row = -1; row < 2; row++)
        {
            //continue if row is out of bounds
            if ((kingRow + row) > 7 || (kingRow + row) < 0) continue;
            for (int col = -1; col < 2; col++)
            {
                //continue if col is out of bounds
                if ((kingCol + col) > 7 || (kingCol + col) < 0) continue;
                if(b[(kingRow + row), (kingCol + col)] == ((opponentColor == Color.White) ? Piece.WhiteKing : Piece.BlackKing))
                {
                    return true;
                }
            }
        }
        //check a knight move away for enemy knights
        int[] knightMoves = { -2, -1, 1, 2 };
        foreach (int dr in knightMoves)
        {
            foreach (int dc in knightMoves)
            {
                if (Math.Abs(dr) + Math.Abs(dc) == 3) //valid knight L-shape
                {
                    int nr = kingRow + dr;
                    int nc = kingCol + dc;

                    if (nr >= 0 && nr < 8 && nc >= 0 && nc < 8)
                    {
                        if (b[nr, nc] == ((opponentColor == Color.White) ? Piece.WhiteKnight : Piece.BlackKnight))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        //check each diagonal until we hit a piece
        //if that piece is a bishop or queen of the opposite color, return true
        int[] diagDr = { -1, -1, 1, 1 };
        int[] diagDc = { -1, 1, -1, 1 };

        for (int d = 0; d < 4; d++)
        {
            int r = kingRow + diagDr[d];
            int c = kingCol + diagDc[d];

            while (r >= 0 && r < 8 && c >= 0 && c < 8)
            {
                Piece p = b[r, c];

                if (p != Piece.Empty)
                {
                    if (p == ((opponentColor == Color.White) ? Piece.WhiteBishop : Piece.BlackBishop) || p == ((opponentColor == Color.White) ? Piece.WhiteQueen : Piece.BlackQueen))
                    {
                        return true;
                    }
                    break; //blocked by another piece
                }

                r += diagDr[d];
                c += diagDc[d];
            }
        }

        //check each file until we hit a piece
        //if that piece is a rook or queen of the opposite color, return true
        int[] direction = { -1, 1};

        for (int d = 0; d < 2; d++)
        {
            //check the rows in the direction
            int i = kingRow + direction[d];
            while (i >= 0 && i < 8)
            {
                if (kingCol < 0 || kingCol >= 8)
                {
                    Debug.Log($"Invalid kingCol: {kingCol}");
                    Debug.Log($"kingRow: {kingRow}");
                }
                Piece p = b[i, kingCol];
                if (p != Piece.Empty)
                {
                    if (p == ((opponentColor == Color.White) ? Piece.WhiteRook : Piece.BlackRook) || p == ((opponentColor == Color.White) ? Piece.WhiteQueen : Piece.BlackQueen))
                    {
                        return true;
                    }
                    break; //blocked
                }

                i += direction[d];
            }

            //check the cols in the direction
            i = kingCol + direction[d];
            while (i >= 0 && i < 8)
            {
                Piece p = b[kingRow, i];
                if (p != Piece.Empty)
                {
                    if (p == ((opponentColor == Color.White) ? Piece.WhiteRook : Piece.BlackRook) || p == ((opponentColor == Color.White) ? Piece.WhiteQueen : Piece.BlackQueen))
                    {
                        return true;
                    }
                    break; //blocked
                }

                i += direction[d];
            }
        }

        //return false if we didnt find any checks
        return false;
    }

    //maps start square to a list of moves that can be taken from that square
    Dictionary<(int, int), List<Move>> generateMoves(Color color)
    {
        Color opponentColor = (color == Color.White) ? Color.Black : Color.White;

        Dictionary<(int, int), List<Move>> movesMap = new  Dictionary<(int, int), List<Move>>();

        Action<int, int> addMovesFromSquare = (row, col) =>
        {
            List<Move> moves = new List<Move>();
            Piece me = board[row, col];
            //if the piece is empty or not my color, ignore it
            if (getPieceColor(me) != color) return;

            //append legal moves
            switch(pieceToString(me)[1]){
                //pawn
                case 'P':{
                    int moveDir = (color == Color.White) ? 1 : -1;
                    //check if it has an empty space in front
                    if (board[row + moveDir, col] == Piece.Empty)
                    {
                        Move moveToAdd = new Move(row, col, row + moveDir, col, me, "");
                        //if this move will promote me
                        if ((color == Color.White && row == 6) || (color == Color.Black && row == 1))
                        {
                            //add all the promotion moves
                            moveToAdd.pawnPromotionTarget = 'Q';
                            moves.Add(moveToAdd);
                            moveToAdd.pawnPromotionTarget = 'B';
                            moves.Add(moveToAdd);
                            moveToAdd.pawnPromotionTarget = 'R';
                            moves.Add(moveToAdd);
                            moveToAdd.pawnPromotionTarget = 'N';
                            moves.Add(moveToAdd);
                            //structs are passed by value so this is fine (since Move is a struct)
                        }
                        else //just add the pawn move
                        {
                            moves.Add(moveToAdd);
                        }

                        //check if it can move 2 forward only if it can move 1 forward
                        bool onCorrectRow = (color == Color.White) ? row == 1 : row == 6;
                        //bool isNotOnLastRank = (color == Color.White) ? row == 6 : row == 1;
                        if (onCorrectRow && board[row + (moveDir * 2), col] == Piece.Empty)
                        {
                            moves.Add(new Move(row, col, row + (moveDir * 2), col, me, ""));
                        }
                    }
                    //check for enemy piece diagonally forward
                    for (int c = -1; c <= 1; c+=2)
                    {
                        if(isInBounds(row + moveDir, col + c))
                        {
                            Move moveToAdd = new Move(row, col, row + moveDir, col + c, me, "");
                            //if the piece there is the opponents
                            bool isKing = board[row + moveDir, col + c] == Piece.WhiteKing || board[row + moveDir, col + c] == Piece.BlackKing;
                            if (getPieceColor(board[row + moveDir, col + c]) == opponentColor && !isKing)
                            {
                                //promotion case
                                if ((color == Color.White && row == 6) || (color == Color.Black && row == 1))
                                {
                                    moveToAdd.pawnPromotionTarget = 'Q';
                                    moves.Add(moveToAdd);
                                    moveToAdd.pawnPromotionTarget = 'B';
                                    moves.Add(moveToAdd);
                                    moveToAdd.pawnPromotionTarget = 'R';
                                    moves.Add(moveToAdd);
                                    moveToAdd.pawnPromotionTarget = 'N';
                                    moves.Add(moveToAdd);
                                }
                                //normal case
                                else
                                {
                                    moves.Add(moveToAdd);
                                }
                            }

                            //en passant
                            if (enPassantDest == (row + moveDir, col + c))
                            {
                                moves.Add(moveToAdd); 
                            }
                        }
                    }
                break;
                }

                //bishop
                case 'B':{
                    int[] diagDr = { -1, -1, 1, 1 };
                    int[] diagDc = { -1, 1, -1, 1 };

                    for (int d = 0; d < 4; d++)
                    {
                        int r = row + diagDr[d];
                        int c = col + diagDc[d];

                        while (r >= 0 && r < 8 && c >= 0 && c < 8)
                        {
                            Piece p = board[r, c];
                            if (p != Piece.Empty)
                            {
                                bool isKing = p == Piece.WhiteKing || p == Piece.BlackKing;
                                if (getPieceColor(p) != color && !isKing)
                                {
                                    //add this move to the list
                                    moves.Add(new Move(row, col, r, c, me, ""));
                                }
                                break; //blocked
                            }

                            moves.Add(new Move(row, col, r, c, me, ""));

                            r += diagDr[d];
                            c += diagDc[d];
                        }
                    }
                if(pieceToString(me)[1] == 'Q') goto case 'R';
                break;
                }

                //rook
                case 'R':{
                    //check each file until we hit a piece
                    int[] direction = { -1, 1};

                    for (int d = 0; d < 2; d++)
                    {
                        //check the rows in the direction
                        int i = row + direction[d];
                        while (i >= 0 && i < 8)
                        {
                            Piece p = board[i, col];
                            if (p != Piece.Empty)
                            {
                                bool isKing = p == Piece.WhiteKing || p == Piece.BlackKing;
                                if (getPieceColor(p) != color && !isKing)
                                {
                                    //add this move to the list
                                    moves.Add(new Move(row, col, i, col, me, ""));
                                }
                                break; //blocked
                            }

                            //add this move to the list
                            moves.Add(new Move(row, col, i, col, me, ""));

                            i += direction[d];
                        }

                        //check the cols in the direction
                        i = col + direction[d];
                        while (i >= 0 && i < 8)
                        {
                            Piece p = board[row, i];
                            if (p != Piece.Empty)
                            {
                                bool isKing = p == Piece.WhiteKing || p == Piece.BlackKing;
                                if (getPieceColor(p) != color && !isKing)
                                {
                                    //add this move to the list
                                    moves.Add(new Move(row, col, row, i, me, ""));
                                }
                                break; //blocked
                            }

                            //add this move to the list
                            moves.Add(new Move(row, col, row, i, me, ""));

                            i += direction[d];
                        }
                    }
                break;
                }

                //queen
                case 'Q':
                    goto case 'B'; //bishop code then goes to rook code if the piece is a queen

                //king
                case 'K':{
                    for (int r = -1; r < 2; r++)
                    {
                        //continue if row is out of bounds
                        if ((row + r) > 7 || (row + r) < 0) continue;
                        for (int c = -1; c < 2; c++)
                        {
                            //continue if col is out of bounds
                            if ((col + c) > 7 || (col + c) < 0) continue;
                            Piece p = board[row + r, col + c];
                            bool isKing = p == Piece.WhiteKing || p == Piece.BlackKing;
                            if(getPieceColor(p) != color && !isKing)
                            {
                                moves.Add(new Move(row, col, row + r, col + c, me, ""));
                            }
                        }
                    }
                    //castling
                    bool hasRightsKingside = (color == Color.White) ? CastlingRights.white_kingside : CastlingRights.black_kingside;
                    bool hasRightsQueenside = (color == Color.White) ? CastlingRights.white_queenside : CastlingRights.black_queenside;
                    if (hasRightsKingside)
                    {
                        bool canCastle = true;
                        //check for pieces between king and rook
                        for (int i = 1; i <= 2; i++)
                        {
                            if (board[row, col + i] != Piece.Empty)
                            {
                                canCastle = false;
                                break;
                            }
                        }
                        //check for checks between king start and end pos, inclusive
                        if(canCastle)
                        for (int i = 0; i <= 2; i++)
                        {
                            Piece[,] tempBoard = new Piece[8, 8];
                            Array.Copy(board, tempBoard, board.Length);
                            
                            teleportPiece(tempBoard, row, col, row, col + i);
                            if(isInCheck(tempBoard, color))
                            {
                                canCastle = false;
                                break;
                            }
                        }

                        if (canCastle)
                        {
                            moves.Add(new Move(row, col, row, col + 2, me, ""));
                        }
                    }
                    if (hasRightsQueenside)
                    {
                        bool canCastle = true;
                        //check for pieces between king and rook
                        for (int i = 1; i <= 3; i++)
                        {
                            if (board[row, col - i] != Piece.Empty)
                            {
                                canCastle = false;
                                break;
                            }
                        }

                        //check for checks between king start and end pos, inclusive
                        if(canCastle)
                        for (int i = 0; i <= 2; i++)
                        {
                            Piece[,] tempBoard = new Piece[8, 8];
                            Array.Copy(board, tempBoard, board.Length);

                            teleportPiece(tempBoard, row, col, row, col - i);
                            if(isInCheck(tempBoard, color))
                            {
                                canCastle = false;
                                break;
                            }
                        }

                        if (canCastle)
                        {
                            moves.Add(new Move(row, col, row, col - 2, me, ""));
                        }
                    }
                break;
                }
                //knight
                case 'N':{
                    int[] knightDr = { -2, -2, -1, -1,  1,  1,  2,  2 };
                    int[] knightDc = { -1,  1, -2,  2, -2,  2, -1,  1 };

                    for (int i = 0; i < 8; i++)
                    {
                        int r = row + knightDr[i];
                        int c = col + knightDc[i];

                        if (!isInBounds(r, c)) continue;

                        Piece p = board[r, c];

                        if (p == Piece.Empty)
                        {
                            moves.Add(new Move(row, col, r, c, me, ""));
                        }
                        else
                        {
                            if (getPieceColor(p) == opponentColor &&
                                p != Piece.WhiteKing && p != Piece.BlackKing)
                            {
                                moves.Add(new Move(row, col, r, c, me, ""));
                            }
                        }
                    }
                break;
                }
            }

            //add the moves found to the moves map
            movesMap[(row, col)] = moves;
        };

        //for every cell on the board
        for(int rows = 0; rows < 8; rows++){
            for(int cols = 0; cols < 8; cols++){
                //add the moves of that cell
                addMovesFromSquare(rows, cols);
            }
        }

        //for every move in movesMap
        foreach (var entry in movesMap)
        {
            //if we make that move on an example board and it results in check for our color, remove it from the list
            entry.Value.RemoveAll(move =>
            {
                Piece[,] tempBoard = new Piece[8, 8];
                Array.Copy(board, tempBoard, board.Length);
                teleportPiece(tempBoard, move.startRow, move.startCol, move.destRow, move.destCol);
                return isInCheck(tempBoard, color);
            });
        }

        return movesMap;
    }
}
