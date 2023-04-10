using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class Minimax {
        internal static Move FindBestMove(Board board, int depth) {
            Move[] moves = MoveGen.GetLegalMoves(board, Core.eColor);
            Move[] bestmoves = new Move[218];
            double highestEval = int.MinValue;
            int counter = 0;

            for (int i = 0; i < moves.Length; i++) {
                Board temp = Board.Clone(board);
                temp.PerformMove(moves[i]);

                double eval = (double)Search(temp, depth, false, int.MinValue, int.MaxValue) / 100;
                if (eval > highestEval) {
                    highestEval = eval;
                    counter = 0;
                    bestmoves[counter++] = moves[i];
                } else if (eval == highestEval) bestmoves[counter++] = moves[i];
                        
                Console.WriteLine($"{moves[i].start} {moves[i].end} {eval}");
            }
            return bestmoves[new Random().Next(0, counter)];
        }

        internal static int Search(Board board, int depth, bool maximizing, int alpha, int beta) {
            if (TranspositionTable.TryRetrieveEval(board, depth, out int value)) {
                Core.transpositions++;
                return value;
            }

            if (depth == 0) {
                Core.nodes++;
                value = Evaluation.Evaluate(board);
                TranspositionTable.Add(board, depth, value);
                return value;
            }

            if (maximizing) {
                value = depth * -50000;
                Board[] children = GetBoardChildren(Board.Clone(board), Core.eColor);
                if (children.Length == 0 && !Core.IsCheck(Board.Clone(board), Core.eColor)) value = 420;

                for (byte i = 0; i < children.Length; i++) {
                    int nextSearch = Search(Board.Clone(children[i]), depth - 1, false, alpha, beta);
                    value = value > nextSearch ? value : nextSearch;
                    alpha = alpha > value ? alpha : value;
                    if (beta <= alpha) break;
                }

                TranspositionTable.Add(board, depth, value);
                return value;
            } else {
                value = depth * 50000;
                Board[] children = GetBoardChildren(Board.Clone(board), Core.pColor);
                if (children.Length == 0 && !Core.IsCheck(Board.Clone(board), Core.pColor)) value = -420;

                for (byte i = 0; i < children.Length; i++) {
                    int nextMinimax = Search(Board.Clone(children[i]), depth - 1, true, alpha, beta);
                    value = value < nextMinimax ? value : nextMinimax;
                    beta = beta < value ? beta : value;
                    if (beta <= alpha) break;
                }

                TranspositionTable.Add(board, depth, value);
                return value;
            }
        }

        internal static Board[] GetBoardChildren(Board board, Color color) {

            // TODO: Sort the child boards using the basic evaluation from Evalutation.BasicEval()

            Move[] moves = MoveGen.GetLegalMoves(Board.Clone(board), color);
            Board[] children = new Board[moves.Length];

            for (int i = 0; i < moves.Length; i++) {
                children[i] = Board.Clone(board);
                children[i].PerformMove(moves[i]);
            }

            return children;
        }
    }
}
