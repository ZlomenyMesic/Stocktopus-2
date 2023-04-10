using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class TranspositionTable {
        private static readonly Dictionary<(ulong, ulong, int), int> hashTable = new();

        private static readonly ulong whiteKingsideCastling = 0x0000000000000069;
        private static readonly ulong whiteQueensideCastling = 0x0000000000042000;
        private static readonly ulong blackKingsideCastling = 0x0000000006900000;
        private static readonly ulong blackQueensideCastling = 0x0000000420000000;

        internal static void Add(Board board, int depth, int eval) {
            hashTable[GetBoardHash(board, depth)] = eval;
        }

        internal static bool TryRetrieveEval(Board board, int depth, out int value) {
            return hashTable.TryGetValue(GetBoardHash(board, depth), out value);
        }

        private static (ulong, ulong, int) GetBoardHash(Board board, int depth) {

            // TODO: Better hashing (not Zobrist)

            ulong whiteHash = 0;
            ulong blackHash = 0;

            for (int i = 0; i < 6; i++) {
                whiteHash ^= board.bitboards[0][i];
                blackHash ^= board.bitboards[1][i];
            }

            if (board.canWhiteCastleKingside) whiteHash ^= whiteKingsideCastling;
            if (board.canWhiteCastleQueenside) whiteHash ^= whiteQueensideCastling;
            if (board.canBlackCastleKingside) blackHash ^= blackKingsideCastling;
            if (board.canBlackCastleQueenside) blackHash ^= blackQueensideCastling;

            return (whiteHash, blackHash, depth);
        }

        internal static void Reset() {
            hashTable.Clear();
        }
    }
}
