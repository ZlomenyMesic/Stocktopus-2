using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal class Board {
        internal Bitboard[][] bitboards = new Bitboard[2][];
        internal Piece[] mailbox = new Piece[64];

        internal Bitboard whiteOccupiedSquares;
        internal Bitboard blackOccupiedSquares;

        internal bool canWhiteCastleKingside;
        internal bool canWhiteCastleQueenside;
        internal bool canBlackCastleKingside;
        internal bool canBlackCastleQueenside;

        internal int enPassantSquare;

        internal Board() {
            bitboards[0] = new Bitboard[6];
            bitboards[1] = new Bitboard[6];

            whiteOccupiedSquares = 0x0000000000000000;
            blackOccupiedSquares = 0x0000000000000000;

            for (int i = 0; i < 6; i++) {
                bitboards[0][i] = 0x0000000000000000;
                bitboards[1][i] = 0x0000000000000000;
            }
        }

        internal void PerformMove(Move move) {
            mailbox[move.end] = mailbox[move.start];
            mailbox[move.start] = new Piece(Color.None, PieceType.None);

            Color color = mailbox[move.end].color;
            Bitboard moveBitboard = Squares.Mask[move.start] | Squares.Mask[move.end];

            bitboards[(int)color][move.piece - 1] ^= moveBitboard;

            if (color == Color.White) whiteOccupiedSquares ^= moveBitboard;
            else blackOccupiedSquares ^= moveBitboard;

            if (move.capture != (int)PieceType.None) {
                bitboards[color == Color.White ? (int)Color.Black : (int)Color.White][move.capture - 1] ^= Squares.Mask[move.end];

                if (color == Color.White) blackOccupiedSquares ^= Squares.Mask[move.end];
                else whiteOccupiedSquares ^= Squares.Mask[move.end];
            }
        }

        internal static Board Clone(Board board) {
            Board toReturn = new() {
                bitboards = board.bitboards,
                mailbox = board.mailbox,

                whiteOccupiedSquares = board.whiteOccupiedSquares,
                blackOccupiedSquares = board.blackOccupiedSquares,

                canBlackCastleKingside = board.canBlackCastleKingside,
                canBlackCastleQueenside = board.canBlackCastleQueenside,
                canWhiteCastleKingside = board.canWhiteCastleKingside,
                canWhiteCastleQueenside = board.canWhiteCastleQueenside,

                enPassantSquare = board.enPassantSquare
            };

            return toReturn;
        }

        internal static void Print(Board board) {
            // TODO
        }
    }
}
