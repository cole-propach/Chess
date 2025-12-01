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
        //check every move of the opponents color and see if any of those result in capturing our king (dont have to be legal, can check with pinned pieces)
        bool result = false;

        return result;
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

        return moves;
    }
}
