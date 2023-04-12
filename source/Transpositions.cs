using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class TranspositionTable {
        private static readonly Dictionary<string, int> hashTable = new();

        internal static void Add(Board board, int depth, int eval) {
            hashTable[GetBoardRepresentation(board, depth)] = eval;
        }

        internal static bool TryRetrieveEval(Board board, int depth, out int eval) {
            return hashTable.TryGetValue(GetBoardRepresentation(board, depth), out eval);
        }

        private static string GetBoardRepresentation(Board board, int depth) {
            StringBuilder result = new();

            for (int i = 0; i < 64; i++) {
                switch ((byte)board.mailbox[i].pieceType) {
                    case 0: result.Append('0'); break;
                    case 1: result.Append(board.mailbox[i].color == Color.White ? 'P' : 'p'); break;
                    case 2: result.Append(board.mailbox[i].color == Color.White ? 'N' : 'n'); break;
                    case 3: result.Append(board.mailbox[i].color == Color.White ? 'B' : 'b'); break;
                    case 4: result.Append(board.mailbox[i].color == Color.White ? 'R' : 'r'); break;
                    case 5: result.Append(board.mailbox[i].color == Color.White ? 'Q' : 'q'); break;
                    case 6: result.Append(board.mailbox[i].color == Color.White ? 'K' : 'k'); break;
                }
            }

            result.Append(board.canWhiteCastleKingside ? 't' : 'f');
            result.Append(board.canWhiteCastleQueenside ? 't' : 'f');
            result.Append(board.canBlackCastleKingside ? 't' : 'f');
            result.Append(board.canBlackCastleQueenside ? 't' : 'f' );

            result.Append(depth * board.enPassantSquare);

            return result.ToString();
        }

        internal static void Reset() {
            hashTable.Clear();
        }
    }
}
