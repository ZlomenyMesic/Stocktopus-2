using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class Minimax {
        internal static Move FindBestMove(Board board, int depth) {
            Move[] moves = MoveGen.GetLegalMoves(board, Core.eColor);
            Move bestmove = new Move(0, 0, 0, 0, 0);
            int highestEval = int.MinValue;

            for (int i = 0; i < moves.Length; i++) {
                Board temp = Board.Clone(board);
                temp.PerformMove(moves[i]);

                int eval = Search(temp, depth, false, int.MinValue, int.MaxValue);
                if (eval > highestEval) {
                    highestEval = eval;
                    bestmove = moves[i];
                }
            }
            return bestmove;
        }

        internal static int Search(Board board, int depth, bool maximizing, int alpha, int beta) {
            if (depth == 0) {
                Core.nodes++;
                int eval = Evaluate(board);
                return eval;
            }
            if (maximizing) {
                int value = depth * -50000;
                Board[] children = GetBoardChildren(board, false);
                if (children.Length == 0 && !Core.IsCheck(board, Core.eColor)) value = 420;

                for (byte i = 0; i < children.Length; i++) {
                    int nextSearch = Search(Board.Clone(children[i]), depth - 1, false, alpha, beta);
                    value = value > nextSearch ? value : nextSearch;
                    alpha = alpha > value ? alpha : value;
                    if (beta <= alpha) break;
                }
                return value;
            } else {
                int value = depth * 50000;
                Board[] children = GetBoardChildren(board, true);
                if (children.Length == 0 && !Core.IsCheck(board, Core.pColor)) value = -420;

                //Console.WriteLine($"{children.Length}");

                for (byte i = 0; i < children.Length; i++) {
                    int nextMinimax = Search(Board.Clone(children[i]), depth - 1, true, alpha, beta);
                    value = value < nextMinimax ? value : nextMinimax;
                    beta = beta < value ? beta : value;
                    if (beta <= alpha) break;
                }
                return value;
            }
        }

        internal static Board[] GetBoardChildren(Board board, bool maximizing) {
            Move[] moves = MoveGen.GetLegalMoves(board, maximizing ? Core.pColor : Core.eColor);
            Board[] children = new Board[moves.Length];

            for (int i = 0; i < moves.Length; i++) {
                children[i] = Board.Clone(board);
                children[i].PerformMove(moves[i]);
            }

            return children;
        }

        internal static int Evaluate(Board board) {
            int eval = 0;
            for (int i = 0; i < 64; i++) {
                int rev = board.mailbox[i].color == Core.eColor ? 1 : -1;
                switch ((byte)board.mailbox[i].pieceType) {
                    case 1: eval += 1 * rev; break;
                    case 2: eval += 3 * rev; break;
                    case 3: eval += 3 * rev; break;
                    case 4: eval += 5 * rev; break;
                    case 5: eval += 9 * rev; break;
                }
            }
            return eval;
        }
    }
}
