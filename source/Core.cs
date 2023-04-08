using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Stocktopus_2 {
    internal static class Core {
        internal static Board board = new Board();

        internal static Color eColor = Color.White;
        internal static Color pColor = Color.Black;

        internal static int nodes = 0;
        internal static int transpositions = 0;

        internal static void Initialize() {
            MoveGen.InitializeKingAttacks();
            MoveGen.InitializeRankAttacks();
            MoveGen.InitializeFileAttacks();
            MoveGen.InitializeA1H8DiagonalAttacks();
            MoveGen.InitializeH1A8DiagonalAttacks();
        }

        internal static void UCINewgame() {
            nodes = 0;
            transpositions = 0;
        }

        internal static void SetPosition(string[] args) {
            board.bitboards[1][0] = 0x000000000000FF00;
            board.bitboards[1][1] = 0x0000000000000042;
            board.bitboards[1][2] = 0x0000000000000024;
            board.bitboards[1][3] = 0x0000000000000081;
            board.bitboards[1][4] = 0x0000000000000008;
            board.bitboards[1][5] = 0x0000000000000010;

            board.bitboards[0][0] = 0x00FF000000000000;
            board.bitboards[0][1] = 0x4200000000000000;
            board.bitboards[0][2] = 0x2400000000000000;
            board.bitboards[0][3] = 0x8100000000000000;
            board.bitboards[0][4] = 0x0800000000000000;
            board.bitboards[0][5] = 0x1000000000000000;

            for (int i = 0; i < 64; i++) {
                board.mailbox[i] = new Piece(Color.None, PieceType.None);
            }

            board.mailbox[0] = new Piece(Color.Black, PieceType.Rook);
            board.mailbox[1] = new Piece(Color.Black, PieceType.Knight);
            board.mailbox[2] = new Piece(Color.Black, PieceType.Bishop);
            board.mailbox[3] = new Piece(Color.Black, PieceType.Queen);
            board.mailbox[4] = new Piece(Color.Black, PieceType.King);
            board.mailbox[5] = new Piece(Color.Black, PieceType.Bishop);
            board.mailbox[6] = new Piece(Color.Black, PieceType.Knight);
            board.mailbox[7] = new Piece(Color.Black, PieceType.Rook);

            board.mailbox[8] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[9] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[10] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[11] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[12] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[13] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[14] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[15] = new Piece(Color.Black, PieceType.Pawn);

            board.mailbox[56] = new Piece(Color.White, PieceType.Rook);
            board.mailbox[57] = new Piece(Color.White, PieceType.Knight);
            board.mailbox[58] = new Piece(Color.White, PieceType.Bishop);
            board.mailbox[59] = new Piece(Color.White, PieceType.Queen);
            board.mailbox[60] = new Piece(Color.White, PieceType.King);
            board.mailbox[61] = new Piece(Color.White, PieceType.Bishop);
            board.mailbox[62] = new Piece(Color.White, PieceType.Knight);
            board.mailbox[63] = new Piece(Color.White, PieceType.Rook);

            board.mailbox[48] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[49] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[50] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[51] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[52] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[53] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[54] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[55] = new Piece(Color.White, PieceType.Pawn);

            board.UpdateGenericBitboards();

            if (args.Length > 3) {
                int moveCount = 0;
                for (int i = 0; i < args.Length - 3; i++) {
                    moveCount++;
                    string startStr = args[i + 3].Substring(0, 2);
                    string endStr = args[i + 3].Substring(2, 2);

                    byte startX = (byte)((byte)"abcdefgh".IndexOf(startStr[0]) + 1);
                    byte startY = (byte)(9 - byte.Parse(startStr[1].ToString()));
                    byte endX = (byte)((byte)"abcdefgh".IndexOf(endStr[0]) + 1);
                    byte endY = (byte)(9 - byte.Parse(endStr[1].ToString()));

                    byte start = (byte)(((startY - 1) * 8) + startX - 1);
                    byte end = (byte)(((endY - 1) * 8) + endX - 1);
                    byte prom = args[i + 3].Length == 5
                        ? (byte)("nbrq".IndexOf(args[i + 3][4]) + 2)
                        : (byte)PieceType.None;

                    bool isCastling = false;
                    bool isEnPassant = false;
                    if ((byte)board.mailbox[start].pieceType == 1 && (start % 8) + 1 != (end % 8) + 1 && board.mailbox[end].pieceType == 0) isEnPassant = true;
                    if ((byte)board.mailbox[start].pieceType == (byte)PieceType.King) {
                        if ((start == 4 && (end == 2 || end == 6)) || (start == 60 && (end == 58 || end == 62))) isCastling = true;
                    }

                    board.PerformMove(new Move(start, end, (byte)board.mailbox[start].pieceType, isEnPassant ? (byte)1 : (byte)board.mailbox[end].pieceType, prom, isCastling, isEnPassant));
                }
                if (moveCount % 2 == 0) {
                    eColor = Color.White;
                    pColor = Color.Black;
                } else {
                    eColor = Color.Black;
                    pColor = Color.White;
                }
            }
        }

        internal static string UCIBestmove() {
            Move pick = Minimax.FindBestMove(board, 3);
            board.PerformMove(pick);
            Console.WriteLine(nodes);
            nodes = 0;

            return $"bestmove {pick}";
        }

        internal static bool IsCheck(Board inpboard, Color kingColor) {
            Move[] moves;

            for (int i = 0; i < 5; i++) {
                moves = new Move[64];
                int j = 0;

                if (i == 0) {
                    MoveGen.GetKingMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 6 || moves[k].capture == 5) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 1) {
                    MoveGen.GetPawnMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 1) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 2) {
                    MoveGen.GetKnightMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 2) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 3) {
                    MoveGen.GetRookMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 4 || moves[k].capture == 5) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 4) {
                    MoveGen.GetBishopMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 3 || moves[k].capture == 5) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                }
            }

            return false;
        }

        internal static bool IsMoveLegal(Board inpboard, Move move, Color color) {
            Board temp = Board.Clone(inpboard);

            bool isCastlingLegal = true;

            if (move.isCastling) {
                if (move.end == 2 && !IsMoveLegal(temp, new Move(4, 3, 6, 0, 0, false), color)) isCastlingLegal = false;
                else if (move.end == 6 && !IsMoveLegal(temp, new Move(4, 5, 6, 0, 0, false), color)) isCastlingLegal = false;
                else if (move.end == 58 && !IsMoveLegal(temp, new Move(60, 59, 6, 0, 0, false), color)) isCastlingLegal = false;
                else if (move.end == 62 && !IsMoveLegal(temp, new Move(60, 61, 6, 0, 0, false), color)) isCastlingLegal = false;
            }

            temp.PerformMove(move);

            return !IsCheck(temp, color) && isCastlingLegal;
        }
    }
}
