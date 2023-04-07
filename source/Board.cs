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

        internal bool canWhiteCastleKingside = true;
        internal bool canWhiteCastleQueenside = true;
        internal bool canBlackCastleKingside = true;
        internal bool canBlackCastleQueenside = true;

        internal int enPassantSquare = -1;

        internal Board() {
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

            if (move.piece == 1 && color == Color.White && move.start >= 48 && move.start <= 55 && move.end >= 32 && move.end <= 39)
                enPassantSquare = move.end;
            else if (move.piece == 1 && color == Color.Black && move.start <= 15 && move.start >= 8 && move.end >= 24 && move.end <= 31)
                enPassantSquare = move.end;
            else enPassantSquare = -1;

            if (move.capture != (byte)PieceType.None) {
                if (!move.isEnPassant) {
                    bitboards[color == Color.White
                        ? (byte)Color.Black
                        : (byte)Color.White][move.capture - 1] ^= Constants.SquareMask[move.end];

                    if (color == Color.White) blackOccupiedSquares ^= Constants.SquareMask[move.end];
                    else whiteOccupiedSquares ^= Constants.SquareMask[move.end];

                    emptySquares ^= Constants.SquareMask[move.start];
                } else {
                    ulong enPassant = color == Color.White
                        ? Compass.South(Constants.SquareMask[move.end])
                        : Compass.North(Constants.SquareMask[move.end]);

                    bitboards[color == Color.White
                        ? (byte)Color.Black
                        : (byte)Color.White][0] ^= enPassant;

                    if (color == Color.White) blackOccupiedSquares ^= enPassant;
                    else whiteOccupiedSquares ^= enPassant;

                    mailbox[move.end + (color == Color.White ? 8 : -8)] = new Piece(Color.None, PieceType.None);
                    emptySquares ^= enPassant;
                }
            } else emptySquares ^= moveBitboard;

            if (move.promotion != 0) {
                bitboards[(byte)color][0] ^= Constants.SquareMask[move.end];

                if (move.promotion == 2) {
                    mailbox[move.end] = new Piece(color, PieceType.Knight);
                    bitboards[(byte)color][1] |= Constants.SquareMask[move.end];
                } 
                else if (move.promotion == 3) {
                    mailbox[move.end] = new Piece(color, PieceType.Bishop);
                    bitboards[(byte)color][2] |= Constants.SquareMask[move.end];
                } 
                else if (move.promotion == 4) {
                    mailbox[move.end] = new Piece(color, PieceType.Rook);
                    bitboards[(byte)color][3] |= Constants.SquareMask[move.end];
                } 
                else if (move.promotion == 5) {
                    mailbox[move.end] = new Piece(color, PieceType.Queen);
                    bitboards[(byte)color][4] |= Constants.SquareMask[move.end];
                }
            }

            if (move.isCastling) {
                Console.WriteLine($"castling {move.start} {move.end}");
                if (move.end == 2) {
                    ulong blackQueenside = 0x0000000000000009;
                    bitboards[1][3] ^= blackQueenside;
                    blackOccupiedSquares ^= blackQueenside;
                    emptySquares ^= blackQueenside;
                    mailbox[0] = new Piece(Color.None, PieceType.None);
                    mailbox[3] = new Piece(Color.Black, PieceType.Rook);
                } 
                else if (move.end == 6) {
                    ulong blackKingside = 0x0000000000000050;
                    bitboards[1][3] ^= blackKingside;
                    blackOccupiedSquares ^= blackKingside;
                    emptySquares ^= blackKingside;
                    mailbox[7] = new Piece(Color.None, PieceType.None);
                    mailbox[5] = new Piece(Color.Black, PieceType.Rook);
                } 
                else if (move.end == 58) {
                    ulong whiteQueenside = 0x0900000000000000;
                    bitboards[0][3] ^= whiteQueenside;
                    whiteOccupiedSquares ^= whiteQueenside;
                    emptySquares ^= whiteQueenside;
                    mailbox[56] = new Piece(Color.None, PieceType.None);
                    mailbox[59] = new Piece(Color.White, PieceType.Rook);
                } 
                else if (move.end == 62) {
                    ulong whiteKingside = 0x5000000000000000;
                    bitboards[0][3] ^= whiteKingside;
                    whiteOccupiedSquares ^= whiteKingside;
                    emptySquares ^= whiteKingside;
                    mailbox[63] = new Piece(Color.None, PieceType.None);
                    mailbox[61] = new Piece(Color.White, PieceType.Rook);
                }
            }

            if (canBlackCastleQueenside && (move.start == 0 || move.end == 0)) canBlackCastleQueenside = false;
            else if (canWhiteCastleKingside && (move.start == 7 || move.end == 7)) canBlackCastleKingside = false;
            else if (canWhiteCastleQueenside && (move.start == 56 || move.end == 56)) canWhiteCastleQueenside = false;
            else if (canWhiteCastleKingside && (move.start == 63 || move.end == 63)) canWhiteCastleKingside = false;

            if (move.start == 4) {
                canBlackCastleQueenside = false;
                canBlackCastleKingside = false;
            } else if (move.start == 60) {
                canWhiteCastleQueenside = false;
                canWhiteCastleKingside = false;
            }

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
