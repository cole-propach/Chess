using UnityEngine;
using System;
using System.Collections.Generic;

public class board_controller : MonoBehaviour
{
    enum Piece {
        Empty,
        WhitePawn, WhiteKnight, WhiteBishop, WhiteRook, WhiteQueen, WhiteKing,
        BlackPawn, BlackKnight, BlackBishop, BlackRook, BlackQueen, BlackKing
    }

    //  (0, 0) is bottom left, or a1
    //  (7, 7) is top right, or h8
    Piece[,] board = new Piece[8, 8];

    void Start()
    {
        //put every piece on the board
        populateBoard();
        printBoard();
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

    string pieceToString(Piece p)
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

    void printBoard()
    {
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

    void makeMove(int startRow, int startCol, int destRow, int destCol)
    {
        board[destRow, destCol] = board[startRow, startCol];
        board[startRow, startCol] = Piece.Empty;
        //check for checkmate or stalemate
    }

    struct Move{
        public int startRow;
        public int startCol;
        public int destRow;
        public int destCol;
        public Piece piece;
        public string name;
    }

    enum Color
    {
        White, Black, Empty
    }

    Color getPieceColor(Piece p)
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
            //break if row is out of bounds
            if ((kingRow + row) > 7 || (kingRow + row) < 0) break;
            for (int col = -1; col < 2; col++)
            {
                //break if col is out of bounds
                if ((kingCol + col) > 7 || (kingCol + col) < 0) break;
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

    List<Move> generateMoves(Color color)
    {
        Color opponentColor = (color == Color.White) ? Color.Black : Color.White;

        List<Move> moves = new List<Move>();

        Action<int, int> appendMoves = (row, col) =>
        {
            Piece me = board[row, col];
            //if the piece is empty or not my color, ignore it
            if (getPieceColor(me) != color)
            {
                return;
            }

            //append legal moves
            switch(pieceToString(me)[1]){
                //pawn
                case 'P':
                    //check if it has an empty space in front
                    //check for enemy piece diagonally forward
                    //en passant
                    //promotion
                break;

                //bishop
                case 'B':

                break;

                //knight
                case 'N':

                break;

                //rook
                case 'R':

                break;

                //queen
                case 'Q':

                break;

                //king
                case 'K':

                break;
            }

        };

        //for every cell on the board
            //append the moves of that cell
        //for every move in moves
            //if we make that move on an example board and it results in check for our color, remove it from the list

        return moves;
    }
}
