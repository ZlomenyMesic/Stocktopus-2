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
        internal Bitboard emptySquares;

        internal bool canWhiteCastleKingside;
        internal bool canWhiteCastleQueenside;
        internal bool canBlackCastleKingside;
        internal bool canBlackCastleQueenside;

        internal int enPassantSquare;

        internal Board() {
            MoveGen.InitializeRankAttacks();
            MoveGen.InitializeFileAttacks();
            MoveGen.InitializeDiagonalAttacks();

            bitboards[0] = new Bitboard[6];
            bitboards[1] = new Bitboard[6];

            whiteOccupiedSquares = 0;
            blackOccupiedSquares = 0;
            emptySquares = 0;

            for (int i = 0; i < 6; i++) {
                bitboards[0][i] = 0;
                bitboards[1][i] = 0;
            }
        }

        internal void PerformMove(Move move) {
            mailbox[move.end] = mailbox[move.start];
            mailbox[move.start] = new Piece(Color.None, PieceType.None);

            Color color = mailbox[move.end].color;
            Bitboard moveBitboard = Constants.SquareMask[move.start] | Constants.SquareMask[move.end];
            emptySquares |= moveBitboard;

            bitboards[(byte)color][move.piece - 1] ^= moveBitboard;

            if (color == Color.White) whiteOccupiedSquares ^= moveBitboard;
            else blackOccupiedSquares ^= moveBitboard;

            if (move.capture != (byte)PieceType.None) {
                emptySquares ^= Constants.SquareMask[move.start];
                bitboards[color == Color.White
                    ? (byte)Color.Black
                    : (byte)Color.White][move.capture - 1] ^= Constants.SquareMask[move.end];

                if (color == Color.White) blackOccupiedSquares ^= Constants.SquareMask[move.end];
                else whiteOccupiedSquares ^= Constants.SquareMask[move.end];
            } else emptySquares ^= moveBitboard;

            UpdateGenericBitboards();
        }

        internal void UpdateGenericBitboards() {
            whiteOccupiedSquares = bitboards[0][0] | bitboards[0][1] | bitboards[0][2] | bitboards[0][3] | bitboards[0][4] | bitboards[0][5];
            blackOccupiedSquares = bitboards[1][0] | bitboards[1][1] | bitboards[1][2] | bitboards[1][3] | bitboards[1][4] | bitboards[1][5];
            emptySquares = ~(whiteOccupiedSquares | blackOccupiedSquares);
        }

        internal void Print() {
            for (int i = 0; i < 64; i++) {
                char square = '-';
                switch (mailbox[i].pieceType) {
                    case PieceType.Pawn: square = mailbox[i].color == Color.White ? 'P' : 'p'; break;
                    case PieceType.Knight: square = mailbox[i].color == Color.White ? 'N' : 'n'; break;
                    case PieceType.Bishop: square = mailbox[i].color == Color.White ? 'B' : 'b'; break;
                    case PieceType.Rook: square = mailbox[i].color == Color.White ? 'R' : 'r'; break;
                    case PieceType.Queen: square = mailbox[i].color == Color.White ? 'Q' : 'q'; break;
                    case PieceType.King: square = mailbox[i].color == Color.White ? 'K' : 'k'; break;
                }
                Console.Write($"{square} ");
                if ((i + 1) % 8 == 0 && i != 0) Console.WriteLine();
            }
        }

        internal Board Clone() {
            Board toReturn = new() {
                bitboards = bitboards,
                mailbox = mailbox,

                whiteOccupiedSquares = whiteOccupiedSquares,
                blackOccupiedSquares = blackOccupiedSquares,
                emptySquares = emptySquares,

                canBlackCastleKingside = canBlackCastleKingside,
                canBlackCastleQueenside = canBlackCastleQueenside,
                canWhiteCastleKingside = canWhiteCastleKingside,
                canWhiteCastleQueenside = canWhiteCastleQueenside,

                enPassantSquare = enPassantSquare
            };

            return toReturn;
        }
    }
}
