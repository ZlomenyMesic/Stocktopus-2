using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class MoveGen {
        internal static void GetPawnMoves(Bitboard pawns, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (pawns != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref pawns);
                targets = GetPawnTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);

                    if ((end < 8 && color == Color.White) || (end > 55 && color == Color.Black)) {
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 2, false);
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 3, false);
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 4, false);
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 5, false);
                    } else moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 0, false);
                }
            }
        }
        internal static Bitboard GetPawnTargets(Bitboard pawns, Board board, Color color) {
            //Bitboard singleStepPushPawns = color == Color.White
            //    ? Compass.South(board.emptySquares) & pawns
            //    : Compass.North(board.emptySquares) & pawns;

            //Bitboard doubleStepPushPawns = color == Color.White
            //    ? Compass.South(Compass.South(board.emptySquares & 0x00000000FF000000) & board.emptySquares) & pawns
            //    : Compass.North(Compass.North(board.emptySquares & 0x000000FF00000000) & board.emptySquares) & pawns;

            Bitboard singleStepPushTargets = color == Color.White
                ? Compass.North(pawns) & board.emptySquares
                : Compass.South(pawns) & board.emptySquares;

            Bitboard doubleStepPushTargets = color == Color.White
                ? Compass.North(singleStepPushTargets) & board.emptySquares & 0x000000FF00000000
                : Compass.South(singleStepPushTargets) & board.emptySquares & 0x00000000FF000000;

            Bitboard westAttacks = color == Color.White
                ? Compass.NorthWest(pawns)
                : Compass.SouthWest(pawns);

            Bitboard eastAttacks = color == Color.White
                ? Compass.NorthEast(pawns)
                : Compass.SouthEast(pawns);

            Bitboard allAttacks = (westAttacks | eastAttacks) & (Core.eColor == Color.White
                ? board.blackOccupiedSquares
                : board.whiteOccupiedSquares);

            // TODO: EN PASSANT

            return allAttacks | singleStepPushTargets | doubleStepPushTargets;
        }
    }
}
