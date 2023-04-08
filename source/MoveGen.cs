using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class MoveGen {
        internal static readonly ulong[] KingAttacks = new ulong[64];

        internal static readonly ulong[][] RankAttacks = new ulong[64][];
        internal static readonly ulong[][] FileAttacks = new ulong[64][];
        internal static readonly ulong[][] A1H8DiagonalAttacks = new ulong[64][];
        internal static readonly ulong[][] H1A8DiagonalAttacks = new ulong[64][];

        internal static void InitializeKingAttacks() {
            for (int i = 0; i < 64; i++) {
                Bitboard king = Constants.SquareMask[i];
                Bitboard attacks = Compass.East(king) | Compass.West(king);
                king |= attacks;
                attacks |= Compass.North(king) | Compass.South(king);
                KingAttacks[i] = attacks;
            }
        }

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

        internal static void InitializeA1H8DiagonalAttacks() {
            for (int i = 0; i < 64; i++) {
                A1H8DiagonalAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    int diag = (sq >> 3) - (sq & 7);
                    ulong targets = 0;
                    ulong rankTargets = diag > 0 ? RankAttacks[sq % 8][occ] : RankAttacks[sq / 8][occ];

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

                    A1H8DiagonalAttacks[sq][occ] = targets;
                }
            }
        }

        internal static void InitializeH1A8DiagonalAttacks() {
            for (int i = 0; i < 64; i++) {
                H1A8DiagonalAttacks[i] = new ulong[64];
            }

            for (int sq = 0; sq < 64; sq++) {
                for (int occ = 0; occ < 64; occ++) {
                    int diag = (sq >> 3) + (sq & 7);
                    ulong targets = 0;
                    ulong rankTargets = diag > 7 ? RankAttacks[7 - sq / 8][occ] : RankAttacks[sq % 8][occ];

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

                    H1A8DiagonalAttacks[sq][occ] = targets;
                }
            }
        }

        internal static void GetPawnMoves(Bitboard pawns, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (pawns != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref pawns);
                targets = Targets.GetPawnTargets(Constants.SquareMask[start], board, color);

                if (board.enPassantSquare != -1) {
                    int difference = (start % 8) - (board.enPassantSquare % 8);
                    if (difference < 2 && difference > -2) {
                        if (start + 1 == board.enPassantSquare)
                            moves[i++] = new Move(start, (byte)(start + (color == Color.White ? -7 : 9)), 1, 1, 0, false, true);
                        else if (start - 1 == board.enPassantSquare)
                            moves[i++] = new Move(start, (byte)(start + (color == Color.White ? -9 : 7)), 1, 1, 0, false, true);
                    }
                }

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);

                    if ((end < 8 && color == Color.White) || (end > 55 && color == Color.Black)) {
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 2);
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 3);
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 4);
                        moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 5);
                    } else moves[i++] = new Move(start, end, 1, (byte)board.mailbox[end].pieceType, 0);
                }
            }
        }

        internal static void GetKnightMoves(Bitboard knights, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (knights != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref knights);
                targets = Targets.GetKnightTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);
                    moves[i++] = new Move(start, end, 2, (byte)board.mailbox[end].pieceType, 0);
                }
            }
        }

        internal static void GetBishopMoves(Bitboard bishops, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (bishops != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref bishops);
                targets = Targets.GetBishopTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);
                    moves[i++] = new Move(start, end, 3, (byte)board.mailbox[end].pieceType, 0);
                }
            }
        }

        internal static void GetRookMoves(Bitboard rooks, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (rooks != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref rooks);
                targets = Targets.GetRookTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);
                    moves[i++] = new Move(start, end, 4, (byte)board.mailbox[end].pieceType, 0);
                }
            }
        }

        internal static void GetQueenMoves(Bitboard queens, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (queens != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref queens);
                targets = Targets.GetRookTargets(Constants.SquareMask[start], board, color) | Targets.GetBishopTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);
                    moves[i++] = new Move(start, end, 5, (byte)board.mailbox[end].pieceType, 0);
                }
            }
        }

        internal static void GetKingMoves(Bitboard king, Board board, Color color, Move[] moves, ref int i) {
            Bitboard targets; byte start; byte end;

            while (king != 0) {
                start = (byte)Bitboard.BitScanForwardReset(ref king);
                targets = Targets.GetKingTargets(Constants.SquareMask[start], board, color);

                while (targets != 0) {
                    end = (byte)Bitboard.BitScanForwardReset(ref targets);
                    moves[i++] = new Move(start, end, 6, (byte)board.mailbox[end].pieceType, 0);
                }
            }
        }

        internal static void GetCastlingMoves(Board board, Color color, Move[] moves, ref int i) {
            if (color == Color.White) {
                if (board.canWhiteCastleKingside && (~board.emptySquares & 0x6000000000000000) == 0)
                    moves[i++] = new Move(60, 62, 6, 0, 0, true);
                if (board.canWhiteCastleQueenside && (~board.emptySquares & 0x0E00000000000000) == 0)
                    moves[i++] = new Move(60, 58, 6, 0, 0, true);
            } else {
                if (board.canBlackCastleKingside && (~board.emptySquares & 0x0000000000000060) == 0)
                    moves[i++] = new Move(4, 6, 6, 0, 0, true);
                if (board.canBlackCastleQueenside && (~board.emptySquares & 0x000000000000000E) == 0)
                    moves[i++] = new Move(4, 2, 6, 0, 0, true);
            }
        }

        internal static void GetAllMoves(Board board, Color color, Move[] moves, ref int i) {
            GetPawnMoves(new Bitboard(board.bitboards[(byte)color][0]), board, color, moves, ref i);
            GetKnightMoves(new Bitboard(board.bitboards[(byte)color][1]), board, color, moves, ref i);
            GetBishopMoves(new Bitboard(board.bitboards[(byte)color][2]), board, color, moves, ref i);
            GetRookMoves(new Bitboard(board.bitboards[(byte)color][3]), board, color, moves, ref i);
            GetQueenMoves(new Bitboard(board.bitboards[(byte)color][4]), board, color, moves, ref i);
            GetKingMoves(new Bitboard(board.bitboards[(byte)color][5]), board, color, moves, ref i);
            GetCastlingMoves(board, color, moves, ref i);
        }

        internal static Move[] GetLegalMoves(Board board, Color color) {
            Move[] allMoves = new Move[300]; 
            Move[] legalMoves = new Move[218];

            int i = 0;
            int j = 0;
            GetAllMoves(board, color, allMoves, ref i);

            for (int k = 0; k < i; k++) {
                if (Core.IsMoveLegal(board, allMoves[k], color)) legalMoves[j++] = allMoves[k];
            }

            Move[] result = new Move[j];
            for (int l = 0; l < j; l++) {
                result[l] = legalMoves[l];
            }

            return result;
        }
    }
}
