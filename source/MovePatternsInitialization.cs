using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class MovePatternsInitialization {
        internal static void InitializeKingAttacks() {
            for (int i = 0; i < 64; i++) {
                Bitboard king = Constants.SquareMask[i];
                Bitboard attacks = Compass.East(king) | Compass.West(king);
                king |= attacks;
                attacks |= Compass.North(king) | Compass.South(king);
                MoveGeneration.KingAttacks[i] = attacks;
            }
        }

        internal static void InitializeRankAttacks() {
            for (int i = 0; i < 64; i++) {
                MoveGeneration.RankAttacks[i] = new ulong[64];
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

                    MoveGeneration.RankAttacks[sq][occ] = targets << (8 * rank);
                }
            }
        }
        internal static void InitializeFileAttacks() {
            for (int i = 0; i < 64; i++) {
                MoveGeneration.FileAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    ulong targets = 0;
                    ulong rankTargets = MoveGeneration.RankAttacks[7 - (sq / 8)][occ];

                    for (int bit = 0; bit < 8; bit++) {
                        int rank = 7 - bit;
                        int file = sq & 7;

                        if (Bitboard.IsBitSet(rankTargets, bit)) {
                            targets |= Constants.SquareMask[file + 8 * rank];
                        }
                    }
                    MoveGeneration.FileAttacks[sq][occ] = targets;
                }
            }
        }

        internal static void InitializeA1H8DiagonalAttacks() {
            for (int i = 0; i < 64; i++) {
                MoveGeneration.A1H8DiagonalAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    int diag = (sq >> 3) - (sq & 7);
                    ulong targets = 0;
                    ulong rankTargets = diag > 0 ? MoveGeneration.RankAttacks[sq % 8][occ] : MoveGeneration.RankAttacks[sq / 8][occ];

                    for (int bit = 0; bit < 8; bit++) {
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

                    MoveGeneration.A1H8DiagonalAttacks[sq][occ] = targets;
                }
            }
        }

        internal static void InitializeH1A8DiagonalAttacks() {
            for (int i = 0; i < 64; i++) {
                MoveGeneration.H1A8DiagonalAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    int diag = (sq >> 3) + (sq & 7);
                    ulong targets = 0;
                    ulong rankTargets = diag > 7 ? MoveGeneration.RankAttacks[7 - sq / 8][occ] : MoveGeneration.RankAttacks[sq % 8][occ];

                    for (int bit = 0; bit < 8; bit++) {
                        int rank; int file;

                        if (Bitboard.IsBitSet(rankTargets, bit)) {
                            if (diag >= 7) {
                                rank = 7 - bit;
                                file = (diag - 7) + bit;
                            } else {
                                rank = diag - bit;
                                file = bit;
                            }
                            if ((file >= 0) && (file <= 7) && (rank >= 0) && (rank <= 7)) {
                                targets |= Constants.SquareMask[file + 8 * rank];
                            }
                        }
                    }

                    MoveGeneration.H1A8DiagonalAttacks[sq][occ] = targets;
                }
            }
        }
    }
}
