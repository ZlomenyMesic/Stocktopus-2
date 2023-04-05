using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class MoveGen {
        internal static ulong[][] RankAttacks = new ulong[64][];
        internal static ulong[][] FileAttacks = new ulong[64][];
        internal static ulong[][] A1H8DiagonalAttacks = new ulong[64][];
        internal static ulong[][] H1A8DiagonalAttacks = new ulong[64][];

        internal static void InitializeRankAttacks() {
            for (int i = 0; i < 64; i++) {
                RankAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    int rank = sq >> 3;
                    int file = sq & 7;

                    ulong occupancy = Bitboard.ToBitboard(occ << 1);
                    ulong targets = 0;

                    int blocker = file + 1;
                    while (blocker <= 7) {
                        targets |= Constants.SquareMask[blocker];
                        if (Bitboard.IsBitSet(occupancy, blocker)) break;
                        blocker++;
                    }

                    blocker = file - 1;
                    while (blocker >= 0) {
                        targets |= Constants.SquareMask[blocker];
                        if (Bitboard.IsBitSet(occupancy, blocker)) break;
                        blocker--;
                    }

                    RankAttacks[sq][occ] = targets << (8 * rank);
                }
            }
        }
        internal static void InitializeFileAttacks() {
            for (int i = 0; i < 64; i++) {
                FileAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    ulong targets = 0;
                    ulong rankTargets = RankAttacks[7 - (sq / 8)][occ];

                    for (int bit = 0; bit < 8; bit++) {
                        int rank = 7 - bit;
                        int file = sq & 7;

                        if (Bitboard.IsBitSet(rankTargets, bit)) {
                            targets |= Constants.SquareMask[file + 8 * rank];
                        }
                    }
                    FileAttacks[sq][occ] = targets;
                }
            }
        }

        internal static void InitializeDiagonalAttacks() {
            for (int i = 0; i < 64; i++) {
                A1H8DiagonalAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    int diag = (sq >> 3) - (sq & 7);
                    ulong targets = 0;
                    ulong rankTargets = diag > 0 ? RankAttacks[sq % 8][occ] : RankAttacks[sq / 8][occ];

                    for (int bit = 0; bit < 8; bit++)
                    {
                        int rank, file;

                        if (Bitboard.IsBitSet(rankTargets, bit)) {
                            if (diag >= 0) {
                                rank = diag + bit;
                                file = bit;
                            } else {
                                file = bit - diag;
                                rank = bit;
                            }
                            if ((file >= 0) && (file <= 7) && (rank >= 0) && (rank <= 7)) {
                                targets |= Constants.SquareMask[file + 8 * rank];
                            }
                        }
                    }

                    A1H8DiagonalAttacks[sq][occ] = targets;
                }
            }
        }

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

        private static Bitboard GetPawnTargets(Bitboard pawns, Board board, Color color) {
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

        internal static void GetKnightMoves(Bitboard knights, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (knights != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref knights);
                targets = GetKnightTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);
                    moves[i++] = new Move(start, end, 2, (byte)board.mailbox[end].pieceType, 0, false);
                }
            }
        }

        private static Bitboard GetKnightTargets(Bitboard knights, Board board, Color color) {
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

        internal static void GetBishopMoves(Bitboard bishops, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (bishops != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref bishops);
                targets = GetBishopTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);
                    moves[i++] = new Move(start, end, 3, (byte)board.mailbox[end].pieceType, 0, false);
                }
            }
        }

        private static Bitboard GetBishopTargets(Bitboard bishops, Board board, Color color) {
            Bitboard occupied = ~board.emptySquares;
            Bitboard targets = 0;

            int square = Bitboard.BitScanForward(bishops);

            int diagonal = 7 + (square >> 3) - (square & 7);
            int occupancy = (int)((occupied & Constants.A1H8DiagonalMask[diagonal]) * Constants.A1H8DiagonalMagic[diagonal] >> 56);
            targets |= A1H8DiagonalAttacks[square][(occupancy >> 1) & 63];

            return targets & (color == Color.White
                ? board.blackOccupiedSquares | board.emptySquares
                : board.whiteOccupiedSquares | board.emptySquares);
        }
    }
}
