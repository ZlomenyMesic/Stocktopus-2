using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal class Bitboard {
        private ulong value;

        internal Bitboard(ulong board) {
            value = board;
        }

        public static implicit operator ulong(Bitboard bitboard) {
            return bitboard.value;
        }

        public static implicit operator Bitboard(ulong board) {
            return new Bitboard(board);
        }

        internal static Bitboard ToBitboard(int board) {
            return (ulong)board;
        }

        internal static bool IsBitSet(Bitboard bitboard, int pos) {
            return (bitboard & ((ulong)1 << pos)) != 0;
        }

        internal static int BitScanForward(Bitboard bitboard) {
            return Constants.DeBrujinTable[((ulong)((long)bitboard.value & -(long)bitboard.value) * Constants.DeBrujinValue) >> 58];
        }

        public static int BitScanForwardReset(ref Bitboard bitboard) {
            ulong bb = bitboard.value;
            bitboard.value &= bb - 1;

            return Constants.DeBrujinTable[((ulong)((long)bb & -(long)bb) * Constants.DeBrujinValue) >> 58];
        }
    }
}
