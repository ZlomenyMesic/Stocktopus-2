using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal class Bitboard {
        private ulong value;

        private Bitboard(ulong board) {
            value = board;
        }

        public static implicit operator ulong(Bitboard bitboard) {
            return bitboard.value;
        }

        public static implicit operator Bitboard(ulong board) {
            return new Bitboard(board);
        }

        public static int BitScanForwardReset(ref Bitboard bitboard) {
            ulong bb = bitboard.value;
            bitboard.value &= bb - 1;

            return Constants.DeBrujinTable[((ulong)((long)bb & -(long)bb) * Constants.DeBrujinValue) >> 58];
        }
    }
}
