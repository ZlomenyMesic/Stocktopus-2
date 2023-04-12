using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class Targets {
        internal static Bitboard GetPawnTargets(Bitboard pawns, Board board, Color color) {
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

            Bitboard allAttacks = (westAttacks | eastAttacks) & (color == Color.White
                ? board.blackOccupiedSquares
                : board.whiteOccupiedSquares);

            // TODO: EN PASSANT

            return allAttacks | singleStepPushTargets | doubleStepPushTargets;
        }

        internal static Bitboard GetKnightTargets(Bitboard knights, Board board, Color color) {
            Bitboard east = Compass.East(knights);
            Bitboard west = Compass.West(knights);
            Bitboard targets = (east | west) << 16;
            targets |= (east | west) >> 16;
            east = Compass.East(east);
            west = Compass.West(west);
            targets |= (east | west) << 8;
            targets |= (east | west) >> 8;

            return targets & (color == Color.White
                ? board.blackOccupiedSquares | board.emptySquares
                : board.whiteOccupiedSquares | board.emptySquares);
        }

        internal static Bitboard GetBishopTargets(Bitboard bishops, Board board, Color color) {
            Bitboard occupied = ~board.emptySquares;
            Bitboard targets = 0;

            int square = Bitboard.BitScanForward(bishops);

            int diagonal = 7 + (square >> 3) - (square & 7);
            int occupancy = (int)((occupied & Constants.A1H8DiagonalMask[diagonal]) * Constants.A1H8DiagonalMagic[diagonal] >> 56);
            targets |= MoveGeneration.A1H8DiagonalAttacks[square][(occupancy >> 1) & 63];

            diagonal = (square >> 3) + (square & 7);
            occupancy = (int)((occupied & Constants.H1A8DiagonalMask[diagonal]) * Constants.H1A8DiagonalMagic[diagonal] >> 56);
            targets |= MoveGeneration.H1A8DiagonalAttacks[square][(occupancy >> 1) & 63];

            return targets & (color == Color.White
                ? board.blackOccupiedSquares | board.emptySquares
                : board.whiteOccupiedSquares | board.emptySquares);
        }
        
        internal static Bitboard GetRookTargets(Bitboard rooks, Board board, Color color) {
            Bitboard occupied = board.whiteOccupiedSquares | board.blackOccupiedSquares;
            Bitboard targets = 0;

            int square = Bitboard.BitScanForward(rooks);

            int rank = square >> 3;
            int occupancy = (int)((occupied & Constants.SixBitRankMask[rank]) >> (8 * rank));
            targets |= MoveGeneration.RankAttacks[square][(occupancy >> 1) & 63];

            int file = square & 7;
            occupancy = (int)((occupied & Constants.SixBitFileMask[file]) * Constants.FileMagic[file] >> 56);
            targets |= MoveGeneration.FileAttacks[square][(occupancy >> 1) & 63];

            return targets & (color == Color.White
                ? board.blackOccupiedSquares | board.emptySquares
                : board.whiteOccupiedSquares | board.emptySquares);
        }

        internal static Bitboard GetKingTargets(Bitboard king, Board board, Color color) {
            Bitboard targets = MoveGeneration.KingAttacks[Bitboard.BitScanForward(king)];
            return targets & (color == Color.White
                ? board.blackOccupiedSquares | board.emptySquares
                : board.whiteOccupiedSquares | board.emptySquares);
        }
    }
}
